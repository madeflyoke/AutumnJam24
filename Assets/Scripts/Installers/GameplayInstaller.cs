using Managers;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private GameplayHandler _gameplayHandler;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_gameplayHandler).AsSingle().NonLazy();
        }
    }
}
