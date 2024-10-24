using Audio;
using Managers;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private AudioController _audioController;
        [SerializeField] private GameStateChanger _gameStateChanger;

        public override void InstallBindings()
        {
            Container.BindInstance(_audioController).AsSingle().NonLazy();
            Container.BindInstance(_gameStateChanger).AsSingle().NonLazy();
        }
    }
}
