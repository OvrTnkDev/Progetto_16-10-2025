using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#region INTERFACCE

#endregion

#region SINGLETON - SocialNetwork
///<summary> Singleton: garantisce che una classe abbia una sola istanzae fornisce un punto di accesso globale a essa.</summary>
/// <typeparam name="T">Tipo della classe singleton</typeparam>
public sealed class SocialNetWork : IObservable
{
    List<Utenti> utenti = new List<Utenti>();
    List<Post> post = new List<Post>();

    private List<IObserver> obs = new List<IObserver>();
    private static SocialNetWork _instance;
    private SocialNetWork() { }
    public static SocialNetWork Instance => _instance ??= new SocialNetWork();
    public void AggiungiUtente(Utenti u) => utenti.Add(u);
    public void AggiungiPost(Post p)
    {
        post.Add(p);
        Notify(p);
    }
    
    public List<Utenti> GetUtenti() => utenti;
    public List<Post> GetPost() => post;

    public void Attach(IObserver observer)
    {
        obs.Add(observer);
    }

    public void Detach(IObserver observer)
    {
        obs.Remove(observer);
    }

    public void Notify(Post post)
    {
        foreach (var observer in obs)
        {
            obs.CaricaPost(post);
        }
    }
}
#endregion

#region FACTORY METHOD
public abstract class Utenti : IObserver
{
    public string Nome { get; set; }
    public List<Utenti> Followings = new();
    public List<Post> Posts = new();

    public void Segui(Utenti utente)
    {
        Followings.Add(utente);
        Console.WriteLine($"Lista dei seguiti aggiornata!");
    }

    public void AggiungiPost(Post post)
    {
        Posts.Add(post);
        SocialNetWork.Instance.AggiungiPost(post);
        Console.WriteLine($"Post Pubblicato!");
    }

    public void CaricaPost(Post post)
    {
        if(Followings.Contains(post.Autore))
        {
            Console.WriteLine($"Notifica per {Nome}: {post.Autore.Name} ha pubblicato un nuovo post");
        }
    }

    public abstract string MostraInfo();
}

public class UtenteBase : Utenti
{

    public UtenteBase(string nome)
    {
        Nome = nome;
    }
    public override string MostraInfo()
    {
        return $"Utente Base Creato! Ciao {Nome}";
    }
}

public class UtentePremium : Utenti
{
    public UtentePremium(string nome)
    {
        Nome = nome;
    }

    public override string MostraInfo()
    {
        return $"Utente Premium Creato! Ciao {Nome}";
    }
}

public class UtenteBusiness : Utenti
{
    public UtenteBusiness(string nome)
    {
        Nome = nome;
    }

    public override string MostraInfo()
    {
        return $"Utente Business Creato! Ciao {Nome}";
    }
}

public static class UtentiFactory
{
    public static Utenti CreaUtente(string tipo, string nome)
    {
        switch (tipo.ToLower())
        {
            case "base":
                return new UtenteBase(nome);

            case "premium":
                return new UtentePremium(nome);

            case "business":
                return new UtenteBusiness(nome);

            default:
                Console.WriteLine($"Errore: tipo non valido!");
                return null;
        }
    }
}
#endregion

#region OBSERVER
public interface IObserver
{
    void CaricaPost(Post post);
}

public interface IObservable
{
    void Attach(IObserver obs);
    void Detach(IObserver obs);
    void Notify(Post post);
}
#endregion


#region CLASSI CONCRETE
// 2. ConcreteComponent: oggetto base senza decorazioni
public class Pizza : IPiatto
{
    public string Descrizione() => "Pizza Margherita:\n";
    public string Prepara() => "Preparazione della pizza in corso...\n";
}

public class Hamburger : IPiatto
{
    public string Descrizione() => "Hamburger:\n";
    public string Prepara() => "Preparazione dell'hamburger in corso...\n";
}

public class Insalata : IPiatto
{
    public string Descrizione() => "Insalata Mista:\n";
    public string Prepara() => "Preparazione dell'insalata in corso...\n";
}

public class Chef
{
    private IPreparazioneStrategia _strategia;
    public Chef(IPreparazioneStrategia strategia)
    {
        _strategia = strategia;
    }
    public void PreparaPiatto(IPiatto piatto)
    {
        string descrizione = piatto.Descrizione();
        string preparazione = _strategia.Prepara(descrizione);
        Console.WriteLine(preparazione);
    }
}
#endregion

#region DECORATORE ASTRATTO
// 3. Decorator: classe astratta che implementa IComponent
//    e incapsula un IComponent interno
public abstract class IngredientiExtra : IPiatto
{
    // Riferimento al componente "decorato"
    protected IPiatto _piatto;

    // Costruttore: richiede un componente da decorare
    protected IngredientiExtra(IPiatto piatto)
    {
        _piatto = piatto;
    }

    // Delegazione dell'operazione al componente interno
    public virtual string Descrizione() => _piatto.Descrizione();
    public virtual string Prepara() => _piatto.Prepara();
}
#endregion

#region DECORATORE CONCRETO
// 4. ConcreteDecoratorA: aggiunge comportamento prima e dopo la chiamata
public class ConFormaggio : IngredientiExtra
{
    public ConFormaggio(IPiatto piatto)
        : base(piatto) { }

    public override string Descrizione() => base.Descrizione() + "\n+ Formaggio";
}

public class ConBacon : IngredientiExtra
{
    public ConBacon(IPiatto piatto)
        : base(piatto) { }

    public override string Descrizione() => base.Descrizione() + "\n+ Bacon";
}

public class ConSalsa : IngredientiExtra
{
    public ConSalsa(IPiatto piatto)
        : base(piatto) { }

    public override string Descrizione() => base.Descrizione() + "\n+ Salsa";
}
#endregion

#region FACTORY
public class PiattoFactory
{
    private static PiattoFactory _instance;

    private PiattoFactory() { }

    public static PiattoFactory Instance => _instance ??= new PiattoFactory();

    public IPiatto CreaPiatto(string tipo) => tipo.ToLower().Trim() switch
    {
        "pizza" => new Pizza(),
        "hamburger" => new Hamburger(),
        "insalata" => new Insalata(),
        _ => null
    };
}
#endregion

#region STRATEGY
public interface IFeed
{
    List<Post> getFeed(List<Post> post, Utenti users);
}

public class FeedData : IFeed
{
    public List<Post> getFeed(List<Post> post, Utenti users)
    {
        return post.OrderByDescending(p => p.Data).ToList();
    }
}

public class FeedHashtag : IFeed
{
    private string hashtag;
    public FeedHashtag(string hashtag)
    {
        this.hashtag = hashtag;
    }
        
    public List<Post> getFeed(List<Post> post, Utenti users)
    {
        return post.Where(p => p.Hashtag.Contains(hashtag)).ToList();
    }
}
#endregion

#region MAIN
// Esempio di utilizzo (Client)
class Program
{
    static void Main()
    {
        Console.Clear();
        bool continua = true;

        while (continua)
        {
            Console.WriteLine("=== Benvenuto nella Cucina dello Chef ===");
            Console.WriteLine("Scegli il piatto:");
            Console.WriteLine("1 - Pizza");
            Console.WriteLine("2 - Hamburger");
            Console.WriteLine("3 - Insalata");
            Console.WriteLine("0 - Esci");

            string sceltaBase = Console.ReadLine();
            IPiatto piatto = sceltaBase switch
            {
                "1" => PiattoFactory.Instance.CreaPiatto("pizza"),
                "2" => PiattoFactory.Instance.CreaPiatto("hamburger"),
                "3" => PiattoFactory.Instance.CreaPiatto("insalata"),
                "0" => null,
                _ => null
            };

            if (piatto == null)
            {
                if (sceltaBase == "0")
                {
                    Console.WriteLine("Arrivederci!");
                    break;
                }
                Console.WriteLine("Scelta non valida.\n");
                continue;
            }

            bool piattoTerminato = false;
            while (!piattoTerminato)
            {
                Console.WriteLine("\nAggiungi ingredienti extra:");
                Console.WriteLine("1 - Formaggio");
                Console.WriteLine("2 - Bacon");
                Console.WriteLine("3 - Salsa");
                Console.WriteLine("0 - Nessuna/Finito");

                string sceltaDeco = Console.ReadLine();
                switch (sceltaDeco)
                {
                    case "1":
                        piatto = new ConFormaggio(piatto);
                        break;
                    case "2":
                        piatto = new ConBacon(piatto);
                        break;
                    case "3":
                        piatto = new ConSalsa(piatto);
                        break;
                    case "0":
                        piattoTerminato = true;
                        break;
                    default:
                        Console.WriteLine("Scelta decorazione non valida.");
                        break;
                }
            }

            bool cotturaTerminata = false;
            while (!cotturaTerminata)
            {
                Console.WriteLine("\nAggiungi ingredienti extra:");
                Console.WriteLine("1 - Al Forno");
                Console.WriteLine("2 - Alla Griglia");
                Console.WriteLine("3 - Fritto");
                Console.WriteLine("0 - Nessuna/Finito");

                string sceltaCottura = Console.ReadLine();
                switch (sceltaCottura)
                {
                    case "1":
                        new Chef(new AlForno()).PreparaPiatto(piatto);
                        cotturaTerminata = true;
                        break;
                    case "2":
                        new Chef(new AllaGriglia()).PreparaPiatto(piatto);
                        cotturaTerminata = true;
                        break;
                    case "3":
                        new Chef(new Fritto()).PreparaPiatto(piatto);
                        cotturaTerminata = true;
                        break;
                    case "0":
                        cotturaTerminata = true;
                        break;
                    default:
                        Console.WriteLine($"Scelta non valida");
                        break;
                }
            }

            Console.WriteLine("\nEcco il tuo piatto:");
            Console.WriteLine(piatto.Descrizione());
            Console.WriteLine("\n------------------------------\n");
        }
    }
}
#endregion