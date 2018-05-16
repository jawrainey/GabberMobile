using System.IO;
using GabberPCL.Models;
using SQLite;

namespace GabberPCL
{
    // Singleton and persistent connection to database to avoid expensive
    // opening/closing operations on the database file
    public static class Session
    {
        // ??
        public static JWToken Token;
        // ??
        public static User ActiveUser;
        // ??
        static SQLiteConnection _connection;
        // Used to access platform specific implementations
        public static Interfaces.IPrivatePath PrivatePath;

        public static SQLiteConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SQLiteConnection(Path.Combine(PrivatePath.PrivatePath(), Config.DATABASE_NAME));
                    _connection.CreateTable<User>();
                    _connection.CreateTable<Project>();
                    _connection.CreateTable<Prompt>();
                    _connection.CreateTable<InterviewSession>();
                    _connection.CreateTable<InterviewParticipant>();
                    _connection.CreateTable<InterviewPrompt>();
                }
                return _connection;
            }
        }
    }
}
