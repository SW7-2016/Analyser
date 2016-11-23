using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using edu.stanford.nlp.ling;
using java.util;
using java.io;
using edu.stanford.nlp.pipeline;
using edu.stanford.nlp.util;
using Console = System.Console;

namespace NLPanalyser
{
    class Program
    {
        static void Main(string[] args)
        {
            // Path to the folder with models extracted from `stanford-corenlp-3.6.0-models.jar`
            var jarRoot = @"C:\Repos\stanford-corenlp-3.6.0-models";

            // Text for processing
            //var text = "Kosgi Santosh sent an email to Stanford University. He didn't get a reply.";
            var text = "The card itself is pretty good. For $250 when I bought it, you pretty much get what you payed for. The card comes with dual fans and a cheap-feeling plastic shell, but again, it is $250. It's not the best looking card out there, but for the money, it does run like a champ. Temps got to about 75 Celsius playing Overwatch maxed out, pretty good. Fan noise wasn't really a problem, until I really put the card to the test.  I tested out The Witcher 3. The card ran fine maxed out (Hairworks off) at ~60fps but then the fans started making this weird humming sound. It would make this &#34;VVVvvvVVVvvvVVVvvvVVVvvv&#34; back-and-forth sounding humming noise that didn't sound like a standard fan noise (temps also got to about 78 Celsius. Pretty hot for the card having two fans...). My card may have been defective, but it's also possible this isn't a defect, and the card is simply badly designed. Watch out for that.  Because of this, I can't recommend the card. I haven't heard anyone else talk about this fan issue, and it's possible that my unit was defective, but just watch for it if you have to buy this card. Might want to look at other 1060s if you're in the market for one. The EVGA SC looks to be better than this. For the extra $10, I'd say it's worth it. Even though this has two fans, it still has lower temperatures.";            
            // Annotation pipeline configuration
            var props = new Properties();
            props.setProperty("annotators", "tokenize, ssplit, pos, parse, sentiment");

            // We should change current directory, so StanfordCoreNLP could find all the model files automatically
            var curDir = Environment.CurrentDirectory;
            Directory.SetCurrentDirectory(jarRoot);
            var pipeline = new StanfordCoreNLP(props);
            Directory.SetCurrentDirectory(curDir);

            // Annotation
            var annotation = new Annotation(text);
            pipeline.annotate(annotation);

            foreach ()
            {
                
            }
            // Result - Pretty Print
            using (var stream = new ByteArrayOutputStream())
            {
                pipeline.prettyPrint(annotation, new PrintWriter(stream));
                Console.WriteLine(stream.toString());
                Console.ReadLine();
                stream.close();
            }
        }
    }
}
