using System.IO;

namespace MapReduce
{
    class Token
    {
        public string Term;
        public int Count;
        public string Doc;
        public int NumContainingDocs;


        public static Token ParseToken(string tokenline)
        {
            var tokenParts = tokenline.Split(' ');
            return new Token {Term = tokenParts[0], Count = int.Parse(tokenParts[1]), Doc = tokenParts[2]};
        }

        public void Write(StreamWriter streamWriter)
        {
            streamWriter.WriteLine($"{Term} {Count} {Doc}");
        }
    }
}