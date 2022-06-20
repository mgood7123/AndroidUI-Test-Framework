using System.Collections.Generic;

namespace AndroidUITestFramework
{
    class NewLineConverter
    {
        System.IO.TextWriter textWriter;

        LinkedList<char> buffer;

        List<(NewLineDetector from, string to)> conversions;
        List<(NewLineDetector from, string to)> eliminated;
        private static object LOCK = new();

        public NewLineConverter(System.IO.TextWriter textWriter)
        {
            this.textWriter = textWriter;
            conversions = new List<(NewLineDetector from, string to)>();
            eliminated = new List<(NewLineDetector from, string to)>();
            buffer = new LinkedList<char>();
        }

        public void addConversion(string from, string to)
        {
            lock (LOCK)
            {
                NewLineDetector conversion = new(textWriter, from);
                conversions.Add((conversion, to));
                conversions.Sort((a, b) =>
                {
                    string sa = a.from.NewLine?.Invoke();
                    int NL_A = sa != null ? sa.Length : 0;

                    string sb = b.from.NewLine?.Invoke();
                    int NL_B = sb != null ? sb.Length : 0;

                    return Comparer<int>.Default.Compare(NL_A, NL_B);
                });
            }
        }

        bool flushing_input = false;

        public (string str, bool isNewLine) processNext(char c)
        {
            lock (LOCK)
            {
                if (flushing_input)
                {
                    // prevent recursion
                    flushing_input = false;
                    return ("" + c, c == '\r');
                }
                if (conversions.Count == 0)
                {
                    // we have no available conversions
                    return ("" + c, false);
                }

                bool bufferScan = false;

                char firstCharacter = c;

                while (true)
                {
                    // loop throuch each detectore
                    // keep detecting until all detectors are exhausted

                    bool matched = false;
                    bool isPartialMatch = false;
                    string replacement = null;

                    foreach ((NewLineDetector newLineDetector, string to) x in conversions)
                    {
                        if (eliminated.Contains(x))
                        {
                            // this detector has been eliminated
                            continue;
                        }
                        if (x.newLineDetector.processNext(firstCharacter))
                        {
                            // we got a complete match
                            matched = true;
                            replacement = "" + x.to;
                            break;
                        }
                        if (x.newLineDetector.getIndex() != 0)
                        {
                            // we got a partial match
                            isPartialMatch = true;
                            continue;
                        }
                        else
                        {
                            // we got no match, eliminate
                            eliminated.Add(x);
                            continue;
                        }
                    }

                    if (matched)
                    {
                        if (!bufferScan)
                        {
                            // we got a match, clear the buffer
                            buffer.Clear();
                            eliminated.Clear();
                        }
                        else
                        {
                            // we got a match, flush it later
                        }
                    }
                    else if (isPartialMatch)
                    {
                        if (!bufferScan)
                        {
                            // we got a partial match
                            buffer.AddLast(firstCharacter);
                            // nothing should be done while we have a partial match
                            return (null, false);
                        }
                        else
                        {
                            // we got a partial match
                            // do nothing and advance to next character
                        }
                    }
                    else
                    {
                        // we got no matches
                        eliminated.Clear();
                        if (!bufferScan && buffer.Count != 0)
                        {
                            buffer.AddLast(firstCharacter);
                        }
                    }

                    if (!bufferScan || !isPartialMatch)
                    {
                        // if !bufferScan
                        // we got a match, or we didnt get any matches at all

                        // if bufferScan
                        // we got a match or we didnt get any matches at all
                        // we cannot reset state during partial match in a bufferScan

                        // reset detector states
                        foreach ((NewLineDetector newLineDetector_, _) in conversions)
                        {
                            newLineDetector_.reset();
                        }
                    }

                    if (matched)
                    {
                        if (!bufferScan)
                        {
                            // we got a match, return its replacement
                            return (replacement, true);
                        }
                        else
                        {
                            // we got a match
                            // flush our replacement to text writer
                            textWriter.Write(replacement);
                        }
                    }

                    if (bufferScan)
                    {
                        // save first character
                        char saved = firstCharacter;

                        // remove first character to proceed to the next
                        buffer.RemoveFirst();

                        // return early if we can
                        if (buffer.Count == 0)
                        {
                            // buffer has become empty, we can no longer match anything
                            return ("" + saved, false);
                        }
                        else
                        {
                            // we still have input in the buffer
                            // flush our input to text writer
                            flushing_input = true;
                            textWriter.Write(saved);

                            // set our input to the first character
                            firstCharacter = buffer.First.Value;
                            continue;
                        }
                    }
                    else
                    {
                        // at this point, we didnt get a match
                        if (buffer.Count == 0)
                        {
                            // buffer is empty, no matches whatsoever
                            return ("" + firstCharacter, false);
                        }

                        // rescan the buffer
                        bufferScan = true;

                        // set our input to the first character
                        firstCharacter = buffer.First.Value;
                        continue;
                    }
                }
            }
        }
    }
}