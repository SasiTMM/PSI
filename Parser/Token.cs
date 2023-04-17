namespace PSI;
using static System.Console;
using static Token.E;

// Represents a PSI language Token
public class Token {
   public Token (Tokenizer source, E kind, string text, int line, int column) 
      => (Source, Kind, Text, Line, Column) = (source, kind, text, line, column);
   public Tokenizer Source { get; }
   public E Kind { get; }
   public string Text { get; }
   public int Line { get; }
   public int Column { get; }

   // The various types of token
   public enum E {
      // Keywords
      PROGRAM, VAR, IF, THEN, WHILE, ELSE, FOR, TO, DOWNTO,
      DO, BEGIN, END, PRINT, TYPE, NOT, OR, AND, MOD, _ENDKEYWORDS,
      // Operators
      ADD, SUB, MUL, DIV, NEQ, LEQ, GEQ, EQ, LT, GT, ASSIGN, 
      _ENDOPERATORS,
      // Punctuation
      SEMI, PERIOD, COMMA, OPEN, CLOSE, COLON, 
      _ENDPUNCTUATION,
      // Others
      IDENT, INTEGER, REAL, BOOLEAN, STRING, CHAR, EOF, ERROR
   }

   // Print a Token
   public override string ToString () => Kind switch {
      EOF or ERROR => Kind.ToString (),
      < _ENDKEYWORDS => $"\u00ab{Kind.ToString ().ToLower ()}\u00bb",
      STRING => $"\"{Text}\"",
      CHAR => $"'{Text}'",
      _ => Text,
   };

   // Utility function used to echo an error to the console
   public void PrintError () {
      if (Kind != ERROR) throw new Exception ("PrintError called on a non-error token");

      OutputEncoding = Encoding.UTF8;

      //save the file data to draw the box line depends on their lengths
      string header = "File: ";
      string linenum = "";
      string lineslen = $"{Source.Lines.Length}"; //max line number length
      string fileName = $"{Source.FileName}";

      WriteLine ($"\n{header}{fileName}");

      //Draw the boxline separator depends on Header length or Lines length
      int separator = (header.Length > lineslen.Length) ? header.Length : lineslen.Length;
      for (int i = 0; i < separator; i++) {
         if (i == separator - 2) Write ("\u252c");
           else Write ("\u2500");
      }

      //complete the boxline depends on the filename length
      for (int i = 0; i < fileName.Length; i++) Write ("\u2500");
      WriteLine ();

      //Write top lines and check for the minimum 2 number of lines
      int top_lines = (Line <= 2) ? Line : 3;
      for (int i = top_lines; i > 0; i--) {
         linenum = $"{Line - i + 1}│";
         for (int j = (separator - 1) - linenum.Length; j > 0; j--) Write (" "); //add space for right justification
         Write ($"{linenum}{Source.Lines[Line - i]}\n");
      }

      //Draw the upper arrow at error point
      ForegroundColor = ConsoleColor.Yellow;
      int arrow_pos = linenum.Length + (separator - 2 - linenum.Length) + Column; //add the spaces also
      for (int i = 0; i < arrow_pos; i++) Write (" ");
      Write ("\u005E\n");

      //Write the error text centre to the arrow
      for (int i = 0; i <= (arrow_pos - Text.Length / 2); i++) Write (" ");
      Write ($"{Text}\n");

      ResetColor ();

      //Write btm lines and check for the minimum 2 number of lines
      int btm_lines = (Source.Lines.Length <= (Line + 1)) ? Source.Lines.Length - Line : 2;
      for (int i = 1; i <= btm_lines; i++) {
         linenum = $"{Line + i}│";
         for (int j = (separator - 1) - linenum.Length; j > 0; j--) Write (" "); //add space for right justification
         Write ($"{linenum}{Source.Lines[Line + i - 1]}\n");
      }
   }

   // Helper used by the parser (maps operator sequences to E values)
   public static List<(E Kind, string Text)> Match = new () {
      (NEQ, "<>"), (LEQ, "<="), (GEQ, ">="), (ASSIGN, ":="), (ADD, "+"),
      (SUB, "-"), (MUL, "*"), (DIV, "/"), (EQ, "="), (LT, "<"),
      (LEQ, "<="), (GT, ">"), (SEMI, ";"), (PERIOD, "."), (COMMA, ","),
      (OPEN, "("), (CLOSE, ")"), (COLON, ":")
   };
}
