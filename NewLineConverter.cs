using System.Collections.Generic;

namespace AndroidUITestFramework
{
    class NewLineConverter
    {
        System.IO.TextWriter textWriter;

        LinkedList<char> buffer;

        List<(NewLineDetector from, string to)> conversions;
        List<(NewLineDetector from, string to)> eliminated;

        public NewLineConverter(System.IO.TextWriter textWriter)
        {
            this.textWriter = textWriter;
            conversions = new List<(NewLineDetector from, string to)>();
            eliminated = new List<(NewLineDetector from, string to)>();
            buffer = new LinkedList<char>();
        }

        public void addConversion(string from, string to)
        {
            NewLineDetector conversion = new NewLineDetector(textWriter, from);
            conversions.Add((conversion, to));
            conversions.Sort((a, b) =>
            {
                string sa = a.from.NewLine?.Invoke();
                int NL_A = sa != null ? sa.Length : 0;

                string sb = b.from.NewLine?.Invoke();
                int NL_B = sb != null ? sb.Length : 0;

                return System.Collections.Generic.Comparer<int>.Default.Compare(NL_A, NL_B);
            });

        }


        public (string str, bool isNewLine) processNext(char c)
        {
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
