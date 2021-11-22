using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Scanner
{
    enum TokenType
    {
        unknown,
        white_space,
        tabulator,
        end_of_line,
        end_of_file,
        id,
        fun_def,
        class_def,          
        open_brace,
        close_brace,
        open_par,
        close_par,
        return_token,
        if_token,
        while_token,
        for_token,
        comment,
        int_number,
        string_const,
        assign,
        or,
        and,
        negation,
        equal,
        not_equal,
        greater,
        greater_or_equal,
        less,
        less_or_equal,
        add,
        add_assign,
        minus,
        minus_assign,
        multiply,
        multiply_assign,
        divide,
        divide_assign,
        divide_mod,
        divide_mod_assign,
        comma,
        dot
    }
    struct Token
    {
        public TokenType type; // one of the token codes from above
        public string stringVal;
        public int intVal;
        public int lineNr;
        public int startPos;
        public int endPos;

        public Token(TokenType type, int intVal, string stringVal, int lineNr, int startPos, int endPos)
        {
            this.type = type;
            this.intVal = intVal;
            this.stringVal = stringVal;
            this.lineNr = lineNr;
            this.startPos = startPos;
            this.endPos = endPos;
        }
        public Token(TokenType type, int intVal, string stringVal)
        {
            this.type = type;
            this.intVal = intVal;
            this.stringVal = stringVal;
            this.lineNr = 0;
            this.startPos = 0;
            this.endPos = 0;
        }
    };
    interface ITokenator
    {
        public Token GetNextToken();
    }
    class Filter : ITokenator
    {
        Scanner scan;
        public Filter(string path)
        {
            scan = new Scanner(path);
        }
        public Filter(Stream st)
        {
            scan = new Scanner(st);
        }
        public Token GetNextToken()
        {
            Token t;
            do
            {
                t = scan.GetNextToken();
            } while (t.type == TokenType.white_space
            || t.type == TokenType.tabulator
            || t.type == TokenType.comment
            || t.type == TokenType.end_of_line);
            return t;
        }

    }

    class Scanner : ITokenator
    {
        Dictionary<string, TokenType> keyWords;
        StreamReader file;
        int lineNr = 1;
        int charNr = 0;
        private void initKeywords()
        {
            keyWords = new Dictionary<string, TokenType>();
            keyWords.Add("if", TokenType.if_token);
            keyWords.Add("while", TokenType.while_token);
            keyWords.Add("for", TokenType.for_token);
            keyWords.Add("class", TokenType.class_def);
            keyWords.Add("def", TokenType.fun_def);
            keyWords.Add("return", TokenType.return_token);
        }
        public Scanner(string path)
        {
            initKeywords();
            try {
                file = new StreamReader(path);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public Scanner(Stream st)
        {
            initKeywords();
            file = new StreamReader(st);     
        }
        public Token GetNextToken()
        {        
            StringBuilder sb = new StringBuilder();
            char c = GetNextChar();
            sb.Append(c);
            int start = charNr, val = 0;         
            TokenType type = TokenType.unknown;
            switch(c)
            {
                case '\r':
                case '\n':
                    type = TokenType.end_of_line;
                    ScanEndOfLine();
                    break;
                case '\t': type = TokenType.tabulator; break;
                case ' ': type = TokenType.white_space; break;
                case ',': type = TokenType.comma; break;
                case '.': type = TokenType.dot; break;
                case '(': type = TokenType.open_par;  break;
                case ')': type = TokenType.close_par; break;
                case '{': type = TokenType.open_brace; break;
                case '}': type = TokenType.close_brace; break;
                case '!':
                    if (ScanIsNextEqual(sb, '=')) type = TokenType.not_equal;
                    else type = TokenType.negation; break;
                case '=':
                    if (ScanIsNextEqual(sb, '=')) type = TokenType.equal;
                    else type = TokenType.assign; break;
                case '>': if (ScanIsNextEqual(sb, '=')) type = TokenType.greater_or_equal;
                    else type = TokenType.greater; break;
                case '<':
                    if (ScanIsNextEqual(sb, '=')) type = TokenType.less_or_equal;
                    else type = TokenType.less; break;
                case '+':
                    if (ScanIsNextEqual(sb, '=')) type = TokenType.add_assign;
                    else type = TokenType.add; break;
                case '-':
                    if (ScanIsNextEqual(sb, '=')) type = TokenType.minus_assign;
                    else type = TokenType.minus; break;
                case '*':
                    if (ScanIsNextEqual(sb, '=')) type = TokenType.multiply_assign;
                    else type = TokenType.multiply; break;
                case '/':
                    if (ScanIsNextEqual(sb, '=')) type = TokenType.divide_assign;
                    else type = TokenType.divide; break;
                case '%':
                    if (ScanIsNextEqual(sb, '=')) type = TokenType.divide_mod_assign;
                    else type = TokenType.divide_mod; break;
                case '&':
                    if (ScanIsNextEqual(sb, '&')) type = TokenType.and;
                    else type = TokenType.unknown; break;
                case '|':
                    if (ScanIsNextEqual(sb, '|')) type = TokenType.or;
                    else type = TokenType.unknown; break;
                case '#': type = TokenType.comment; SkipSingleLineComment(sb); break;
                case '"': type = TokenType.string_const; ScanString(sb); break;
                case (char)0x1A: type = TokenType.end_of_file; break; // end of file
                default:
                    {
                        if (char.IsLetter(c) || c == '_')
                            type = ScanLetters(sb);
                        if (char.IsDigit(c))
                        {
                            type = TokenType.int_number;
                            val = ScanInt(c);
                        }
                    }
                    break;

            }
            int end = charNr;
            return new Token(type, val, sb.ToString(), lineNr, start, end);
        }

        private void SkipSingleLineComment(StringBuilder sb)
        {
            sb.Clear();
            char c = (char)file.Peek();
            while (c != Environment.NewLine[0])
            {
                c = GetNextChar();
                sb.Append(c);
                int intc = file.Peek();         
                c = (char)intc;
                if (intc == -1) break;
            }
        }
        private void ScanString(StringBuilder sb)
        {
            sb.Clear();
            char c = GetNextChar();
            char prev = 'a';
            while (c != '"' || prev == '\\')
            {
                sb.Append(c);
                prev = c;
                c = GetNextChar();
            }
        }
        private int ScanInt(char startc)
        {
            int val = (startc - '0');
            char c = (char)file.Peek();
            while (char.IsDigit(c))
            {
                c = GetNextChar();
                val = val * 10 + (c - '0');
                c = (char)file.Peek();
            }
            return val;
        }
        private bool ScanIsNextEqual(StringBuilder sb, char next)
        {
            char c = (char)file.Peek();
            if (c == next)
            {
                c = GetNextChar();
                sb.Append(c);
                return true;
            }
            return false;
        }

        private TokenType ScanLetters(StringBuilder sb)
        {
            char c = (char)file.Peek();
            while(char.IsLetterOrDigit(c) || c == '_')
            {
                c = GetNextChar();
                sb.Append(c);
                c = (char)file.Peek();
            }
            string s = sb.ToString();
            if (keyWords.ContainsKey(s))
                return keyWords[s];
            return TokenType.id;
        }

        private char GetNextChar()
        {
            //offset += file.Read(b, offset, 1);
            int intc = file.Read();
            char c = (char)intc;
            if (intc == -1)
                c = (char)0x1A;
            charNr++;
            return c;
        }
        private void ScanEndOfLine()
        {
            for (int i = 1; i < Environment.NewLine.Length; i++)
            {
                char c = GetNextChar();
                if (c != Environment.NewLine[i])
                    throw new Exception("OS error");
            }
            lineNr++;
            charNr = 0;
        }
    }
}
