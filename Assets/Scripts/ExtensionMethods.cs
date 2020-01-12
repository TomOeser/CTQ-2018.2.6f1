public static class ExtensionMethods
{
    public static Player GetPlayer(this BoltConnection connection)
    {
        if (connection == null)
        {
            return Player.serverPlayer;
        }

        return (Player)connection.UserData;
    }

    public static BoltConnection GetConnection(this BoltConnection connection)
    {
        return connection;
    }
}