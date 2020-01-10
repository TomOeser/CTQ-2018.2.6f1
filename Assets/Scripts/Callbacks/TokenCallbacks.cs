[BoltGlobalBehaviour]
public class TokenCallbacks : Bolt.GlobalEventListener
{
    public override void BoltStartBegin()
    {
        BoltNetwork.RegisterTokenClass<TestToken>();
    }
}