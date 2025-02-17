// See https://aka.ms/new-console-template for more information
using AnimeDB.Database;
using AnimeDB.Database.Tables;
using AnimeDB.UserInterface;
using AnimeDB.UserInterface.MenuOptions;
using AnimeDB.UserInterface.prompts;


Root root = new Root();

var connection_menu = new NestedMenu(
    new()
    {
        new TextOption("Server", "The server to connect to.", Config.Get().DataSource, (a,m) => {
            Config.Get().DataSource = a;
        }),
        new TextOption("Database", "The database to connect to.", Config.Get().Database, (a,m) => {
            Config.Get().Database = a;
        }),
        new TextOption("Username", "The username.", Config.Get().Name, (a,m) => {
            Config.Get().Name = a;
        }),
        new TextOption("Password", "The password.", Config.Get().Password, (a,m) => {
            Config.Get().Password = a;
        }),
        new ToggleOption(Config.Get().TrustCertificate, "Trust server certificate", "This setting might get you to connect! Or not.", () =>
        {
            Config.Get().TrustCertificate = true;
        }, 
        () =>
        {
            Config.Get().TrustCertificate = false;
        }
        ),
        new LambdaMenuOption("Connect", "Try connecting to the database, incorrect credentials throw you right back here or other failiure. Connection has a timeout of 5 seconds, the application may freeze.", (m) =>
        {
            if(DatabaseSingleton.GetInstance() != null){
                root.ClosePrompt();
                DatabaseController.CheckDatabaseIntegrity(root);
            }
            else root.OpenPrompt(new ErrorPrompt(DatabaseSingleton.LastConnectionFailiureReason));
            
        })
    },
    0
);


connection_menu.Title = "Connection:";
connection_menu.Width = 80;
connection_menu.Height = 20;

var create_user = new MenuBuilder<User>(root).BuildMenu((ep) => { }, () => { });
var create_genre = new MenuBuilder<Genre>(root).BuildMenu((ep) => { }, () => { });
var create_anime = new MenuBuilder<Anime>(root).BuildMenu((ep) => { }, () => { });
var create_movie_episode = new MenuBuilder<MovieEpisode>(root).BuildMenu((ep) => { }, () => { });
var create_watchlist_entry = new MenuBuilder<Watchlist>(root).BuildMenu((ep) => { }, () => { });

string export_filepath = "";
string export_username = "";
var export_menu = new NestedMenu(
    new()
    {
        new TextOption("Filepath", "", export_filepath, (a,m) => {
            export_filepath = a;
        }),
        new TextOption("Username", "", export_username, (a,m) => {
            export_username = a;
        }),
        new LambdaMenuOption("Export", "", (m) =>
        {
            Exports.ExportUserData(export_username, export_filepath, root);
        }),
        new LambdaMenuOption("Exit", "", (a) => { root.ClosePrompt(); })
    },
    0
);

string import_filepath = "";
var import_menu = new NestedMenu(
    new()
    {
        new TextOption("Filepath", "", export_filepath, (a,m) => {
            import_filepath = a;
        }),
        new LambdaMenuOption("Import", "", (m) =>
        {
            Exports.Import(import_filepath, root);
        }),
        new LambdaMenuOption("Exit", "", (a) => { root.ClosePrompt(); })
    },
    0
);

string anime_delete_name = "";
var anime_delete_menu = new NestedMenu(
    new()
    {
        new TextOption("Anime Name", "", anime_delete_name, (a,m) => {
            anime_delete_name = a;
        }),
        new LambdaMenuOption("Delete", "", (m) =>
        {
            if (Anime.Delete(anime_delete_name, root))
            {
                root?.OpenPrompt(new InformationPrompt($"All records of the anime '{anime_delete_name} were successfully removed."));
            }
        }),
        new LambdaMenuOption("Exit", "", (a) => { root.ClosePrompt(); })
    },
    0
);

var new_name = "";
var old_name = "";
var rename_menu = new NestedMenu(
    new()
    {
        new TextOption("Old Name", "", old_name, (a,m) => {
            old_name = a;
        }),
        new TextOption("New Name", "", new_name, (a,m) => {
            new_name = a;
        }),
        new LambdaMenuOption("Rename", "", (m) =>
        {
            if (User.Rename(old_name, new_name, root))
            {
                root?.OpenPrompt(new InformationPrompt($"User successfully renamed."));
            }
        }),
        new LambdaMenuOption("Exit", "", (a) => { root.ClosePrompt(); })
    },
    0
);

var action_menu = new NestedMenu(
    new()
    {
        new LambdaMenuOption("Create User", "", (m) =>
        {
            root.OpenPrompt(create_user);
        }),
        new LambdaMenuOption("Create Genre", "", (m) =>
        {
            root.OpenPrompt(create_genre);
        }),
        new LambdaMenuOption("Create Anime", "", (m) =>
        {
            root.OpenPrompt(create_anime);
        }),
        new LambdaMenuOption("Create Movie/Episode", "", (m) =>
        {
            root.OpenPrompt(create_movie_episode);
        }),
        new LambdaMenuOption("Add Watchlist Entry", "", (m) =>
        {
            root.OpenPrompt(create_watchlist_entry);
        }),
        new LambdaMenuOption("Export User Data", "", (m) =>
        {
            root.OpenPrompt(export_menu);
        }),
        new LambdaMenuOption("Import User Data", "", (m) =>
        {
            root.OpenPrompt(import_menu);
        }),
        new LambdaMenuOption("Delete Anime", "", (m) =>
        {
            root.OpenPrompt(anime_delete_menu);
        }),
        new LambdaMenuOption("Rename User", "", (m) =>
        {
            root.OpenPrompt(rename_menu);
        }),
        new LambdaMenuOption("Reset Database", "", (m) =>
        {
            root.OpenPrompt(new ConfirmationPrompt("This action will completely clear the database and delete all data, Do you really want to do this?", () =>
            {
                DatabaseController.ResetDatabase(root);
            }));
        })
    },
    0
);

action_menu.Title = "Available actions:";
action_menu.X = 0;
action_menu.Y = 0;
action_menu.Width = 40;
action_menu.Height = 30;

root.OpenPrompt(action_menu);

if (DatabaseSingleton.GetInstance() == null) root.OpenPrompt(connection_menu);
else
{
    DatabaseController.CheckDatabaseIntegrity(root);
}
root.Run();

/*
[ X ] Vytvořte uživatelské rozhraní pro Vámi navrženou databázi, která bude mít alespoň 5 tabulek. V rámci databáze musí být minimálně jedna vazba M:N.Mezi atributy musíte použít následující datové typy: čísla(int a float), logickou hodnotu(bool), řetězec(string), datum a čas (datetime). 


[ X ] Vložení, smazání a úpravu nějaké informace, záznamu, který se ukládá do více než jedné tabulky. Například vložení objednávky, která má položky apod.
[ X ] Import dat do minimálně dvou tabulek z formátu CSV, XML nebo JSON (Můžete počítat s tím, že budete importovat data do prázdné databáze)
[ X ] Nastavit celý program v konfiguračním souboru.

[ X ] Programátorskou dokumentaci
[   ] Soubor README
[ X ] V případě všech možných chyb musí program rozumným způsobem reagovat, nebo vyžadovat součinnost uživatele k vyřešení problému. 
[   ] 1x testovací scénář ve formátu PDF (.pdf) pro spuštění aplikace, včetně nastavení a importu databázové struktury.
[   ] Minimálně 2x testovací scénáře ve formátu PDF (.pdf), podle kterých můžeme rozumně otestovat práci vaší aplikace s daty s databáze 
[ X ] Program musí využívat návrhové vzory a "best practice" tam, kde je to vhodné.

[ X ] K vypracování použijte programovací jazyk C# (popř. Javu, Python nebo C++). 
[ X ] Dbejte na to, abyste svůj zdrojový kód nesdíleli s žádným spolužákem. 
[ X ] Uvědomte si, že pokud použijete nějaký z nástrojů AI, mohl by váš kód shodovat s kódem spolužáka. 
[ X ] Nebude-li kód, jehož nejste autory (včetně kódu generovaného AI) řádně oddělen od vašeho autorského kódu, bude vaše práce považována za plagiát a hodnocena známkou 5.

*/