using System;
using System.Collections.Generic;
using System.Text;

namespace accela.Extensions
{
    public static class Slugify
    {
        /// <summary>
        /// Produces optional, URL-friendly version of a title, "like-this-one". 
        /// hand-tuned for speed, reflects performance refactoring contributed
        /// by John Gietzen (user otac0n) 
        /// </summary>
        public static string URLFriendly(string title)
        {
            if (title == null) return "";

            const int maxlen = 80;
            int len = title.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' || 
                    c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if ((int)c >= 128)
                {
                    int prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != sb.Length) prevdash = false;
                }
                if (i == maxlen) break;
            }

            if (prevdash)
                return sb.ToString().Substring(0, sb.Length - 1);
            else
                return sb.ToString();
        }

        public static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("????????????????".Contains(s))
            {
                return "a";
            }
            else if ("??????????".Contains(s))
            {
                return "e";
            }
            else if ("??????????".Contains(s))
            {
                return "i";
            }
            else if ("????????????????".Contains(s))
            {
                return "o";
            }
            else if ("????????????".Contains(s))
            {
                return "u";
            }
            else if ("????????".Contains(s))
            {
                return "c";
            }
            else if ("??????".Contains(s))
            {
                return "z";
            }
            else if ("????????".Contains(s))
            {
                return "s";
            }
            else if ("????".Contains(s))
            {
                return "n";
            }
            else if ("????".Contains(s))
            {
                return "y";
            }
            else if ("????".Contains(s))
            {
                return "g";
            }
            else if (c == '??')
            {
                return "r";
            }
            else if (c == '??')
            {
                return "l";
            }
            else if (c == '??')
            {
                return "d";
            }
            else if (c == '??')
            {
                return "ss";
            }
            else if (c == '??')
            {
                return "th";
            }
            else if (c == '??')
            {
                return "h";
            }
            else if (c == '??')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }
    }
}