using Random = UnityEngine.Random;

namespace Firewall
{
    public class PortsManager : SceneSingleton<PortsManager>
    {
        public Port[] Ports { get; private set; }
        public Port OpenPort {get; private set;}
        protected override void OnAwake()
        {
            Ports = GetComponentsInChildren<Port>();
        }

        private void Start()
        {
            OpenPort = Ports[Random.Range(0, Ports.Length)];
            OpenPort.Open();
        }
    }
}