using PokemonGo.Persistence.Entities;
using PokemonGo.Persistence.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using System.Linq;

namespace PokemonGo.Runtime.BattleFieldSystems
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] InventoryView _view;
        [Inject] IItemRepository _itemRepository;


        private void Start()
        {
            _itemRepository.OnSaved += ViewRefreshSlots;
            ViewRefreshSlots(_itemRepository.GetItems());
        }

        void ViewRefreshSlots(IEnumerable<Item> saved)
        {
            IEnumerable<(int, int)> enumerable = _itemRepository.GetItems()
                                                                .Select(item => (item.Id, item.Num));
            _view.RefreshSlots(enumerable);
        }
    }





    #region Linq 확장함수들 중 쿼리용 체이닝 함수 이해를 돕기위한 예제
    public struct Enumerable<T> : IEnumerable<(int, int)>
    {
        public Enumerable(IEnumerable<T> source, Func<T, (int, int)> func)
        {
            _source = source;
            _func = func;
        }

        IEnumerable<T> _source;
        Func<T, (int, int)> _func;
        public IEnumerator<(int, int)> GetEnumerator()
        {
            return new Enumerator<T>(_source, _func);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct Enumerator<T> : IEnumerator<(int, int)>
    {
        public Enumerator(IEnumerable<T> source, Func<T, (int, int)> func)
        {
            _source = source;
            _sourceEnum = null;
            _func = func;
        }

        public (int, int) Current
        {
            get
            {
                return _func.Invoke(_sourceEnum.Current);
            }
        }

        object IEnumerator.Current => Current;

        IEnumerable<T> _source;
        IEnumerator<T> _sourceEnum;
        Func<T, (int, int)> _func;

        public bool MoveNext()
        {
            if (_sourceEnum == null)
                _sourceEnum = _source.GetEnumerator();

            return _sourceEnum.MoveNext();
        }

        public void Reset()
        {
            if (_sourceEnum == null)
                return;

            _sourceEnum.Reset();
        }
        public void Dispose()
        {
        }
    }
    #endregion
}
