using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Collections.Generic;

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
            observer.CaricaPost(post);
        }
    }
}
#endregion

#region FACTORY METHOD
public abstract class Utenti : IObserver
{
    public string? Nome { get; set; }
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
        if(Followings.Contains(post.Author))
        {
            Console.WriteLine($"Notifica per {Nome}: {post.Author.Nome} ha pubblicato un nuovo post");
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
public class Post
{
    public string Content { get; set; }
    public Utenti Author { get; set; }
    public DateTime Date { get; set; }
    public List<string> Hashtags { get; set; } = new List<string>();

    public Post(Utenti author, string content)
    {
        Author = author;
        Content = content;
        Date = DateTime.Now;
    }

    public virtual void Show()
    {
        Console.WriteLine($"[{Date}] {Author.Nome}: {Content}");
        if (Hashtags.Any())
            Console.WriteLine($"Hashtags: {string.Join(", ", Hashtags)}");
    }
}

public class ImagePost : PostDecorator
{
    public string ImageUrl { get; set; }
    public ImagePost(Post post, string imageUrl) : base(post)
    {
        ImageUrl = imageUrl;
    }

    public override void Show()
    {
        base.Show();
        Console.WriteLine($"[Immagine allegata: {ImageUrl}]");
    }
}

public class TagPost : PostDecorator
{
    public List<string> Tags { get; set; } = new List<string>();
    public TagPost(Post post, List<string> tags) : base(post)
    {
        Tags = tags;
    }

    public override void Show()
    {
        base.Show();
        if (Tags.Any())
            Console.WriteLine($"Tag: {string.Join(", ", Tags)}");
    }
}
#endregion

#region DECORATORE ASTRATTO
public abstract class PostDecorator : Post
{
    protected Post post;
    public PostDecorator(Post post) : base(post.Author, post.Content)
    {
        this.post = post;
    }

    public override void Show()
    {
        post.Show();
    }
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
        return post.OrderByDescending(p => p.Date).ToList();
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
        return post.Where(p => p.Hashtags.Contains(hashtag)).ToList();
    }
}
#endregion

#region MAIN
class Program
{
    static void Main()
    {
        var sn = SocialNetWork.Instance;
        bool continua = true;

        while (continua)
        {
            Console.Clear();
            Console.WriteLine("=== Benvenuto nel Social Network ===");
            Console.WriteLine("1 - Crea utente");
            Console.WriteLine("2 - Segui utente");
            Console.WriteLine("3 - Pubblica post");
            Console.WriteLine("4 - Visualizza feed");
            Console.WriteLine("0 - Esci");

            string scelta = Console.ReadLine();
            switch (scelta)
            {
                case "1":
                    Console.Write("Inserisci il nome utente: ");
                    string nome = Console.ReadLine();
                    Console.Write("Tipo utente (base/premium/business): ");
                    string tipo = Console.ReadLine();
                    var utente = UtentiFactory.CreaUtente(tipo, nome);
                    if (utente != null)
                    {
                        sn.AggiungiUtente(utente);
                        sn.Attach(utente); // per ricevere notifiche
                        Console.WriteLine(utente.MostraInfo());
                    }
                    Console.WriteLine("Premi un tasto per continuare...");
                    Console.ReadKey();
                    break;

                case "2":
                    if (sn.GetUtenti().Count < 2)
                    {
                        Console.WriteLine("Servono almeno 2 utenti per seguire!");
                        Console.ReadKey();
                        break;
                    }

                    Console.WriteLine("Scegli chi segue:");
                    for (int i = 0; i < sn.GetUtenti().Count; i++)
                        Console.WriteLine($"{i} - {sn.GetUtenti()[i].Nome}");
                    int followerIndex = int.Parse(Console.ReadLine());
                    Console.WriteLine("Scegli chi essere seguito:");
                    int followingIndex = int.Parse(Console.ReadLine());

                    var follower = sn.GetUtenti()[followerIndex];
                    var following = sn.GetUtenti()[followingIndex];
                    if (follower != following)
                    {
                        follower.Segui(following);
                        Console.WriteLine($"{follower.Nome} ora segue {following.Nome}");
                    }
                    else
                    {
                        Console.WriteLine("Non puoi seguire te stesso!");
                    }
                    Console.ReadKey();
                    break;

                case "3":
                    if (sn.GetUtenti().Count == 0)
                    {
                        Console.WriteLine("Non ci sono utenti registrati!");
                        Console.ReadKey();
                        break;
                    }

                    Console.WriteLine("Scegli l'autore del post:");
                    for (int i = 0; i < sn.GetUtenti().Count; i++)
                        Console.WriteLine($"{i} - {sn.GetUtenti()[i].Nome}");
                    int autoreIndex = int.Parse(Console.ReadLine());
                    var autore = sn.GetUtenti()[autoreIndex];

                    Console.Write("Scrivi il contenuto del post: ");
                    string contenuto = Console.ReadLine();
                    var post = new Post(autore, contenuto);

                    Console.Write("Vuoi aggiungere hashtag? (s/n): ");
                    if (Console.ReadLine().ToLower() == "s")
                    {
                        Console.Write("Inserisci hashtag separati da spazio: ");
                        post.Hashtags.AddRange(Console.ReadLine().Split(' '));
                    }

                    Console.Write("Vuoi aggiungere un'immagine? (s/n): ");
                    if (Console.ReadLine().ToLower() == "s")
                    {
                        Console.Write("Inserisci URL immagine: ");
                        post = new ImagePost(post, Console.ReadLine());
                    }

                    Console.Write("Vuoi aggiungere tag? (s/n): ");
                    if (Console.ReadLine().ToLower() == "s")
                    {
                        Console.Write("Inserisci tag separati da spazio: ");
                        post = new TagPost(post, Console.ReadLine().Split(' ').ToList());
                    }

                    autore.AggiungiPost(post);
                    Console.WriteLine("Post pubblicato!");
                    Console.ReadKey();
                    break;

                case "4":
                    if (sn.GetPost().Count == 0)
                    {
                        Console.WriteLine("Non ci sono post!");
                        Console.ReadKey();
                        break;
                    }

                    Console.WriteLine("Scegli il tipo di feed:");
                    Console.WriteLine("1 - Ordine per data");
                    Console.WriteLine("2 - Filtra per hashtag");
                    string feedScelta = Console.ReadLine();

                    IFeed feedStrategy;

                    if (feedScelta == "2")
                    {
                        Console.Write("Inserisci hashtag da filtrare (senza #): ");
                        string tag = Console.ReadLine();
                        feedStrategy = new FeedHashtag(tag);
                    }
                    else
                    {
                        feedStrategy = new FeedData();
                    }

                    Console.WriteLine("\nFeed:");
                    var feed = feedStrategy.getFeed(sn.GetPost(), null);
                    foreach (var p in feed)
                        p.Show();

                    Console.WriteLine("\nPremi un tasto per continuare...");
                    Console.ReadKey();
                    break;

                case "0":
                    continua = false;
                    break;

                default:
                    Console.WriteLine("Scelta non valida!");
                    Console.ReadKey();
                    break;
            }
        }
    }
}
    

#endregion