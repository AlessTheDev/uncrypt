namespace BehaviourTree.Core.Strategies
{
    public interface IStrategy
    {
        Node.Status Process() => Node.Status.Running;
        Node.Status FixedProcess() => Node.Status.Running;
        void Reset() {}
        void Start() {}
    }
}