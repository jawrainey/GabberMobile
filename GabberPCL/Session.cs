using System.IO;
using SQLite;

namespace GabberPCL
{
    // Singleton and persistent connection to database to avoid expensive
    // opening/closing operations on the database file
    public static class Session
    {
        static SQLiteConnection _connection;
        // Used to access platform specific implementations
        public static Interfaces.IPrivatePath PrivatePath;

        public static SQLiteConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    // ??
                    _connection = new SQLiteConnection(Path.Combine(PrivatePath.PrivatePath(), "gabber.db3"));
                    // ??
                    _connection.CreateTable<Participant>();
                    return _connection;
                }
                return _connection;
            }
        }
    }
}
