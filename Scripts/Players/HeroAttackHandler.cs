//todod delete
//using System.Collections.Generic;
//using UnityEngine.EventSystems;
//using UnityEngine;
//using System.Linq;

//public class HeroAttackHandler
//{
//    private FieldHero _fieldHero;
//    private HeroAttackCommand _attackCommand;

//    public HeroAttackHandler(FieldHero hero)
//    {
//        _fieldHero = hero;
//    }

//    public void HandleAttack(HeroData heroData, WeaponItem weaponCard)
//    {
//        if (_fieldHero.HeroData != heroData) return;

//        var enemiesInRange = EnemiesInRange(weaponCard.weaponRange)
//            .Where(e => !e.Key.isTargeted && LineOfSight.CheckLos(_fieldHero, e.Key))
//            .ToDictionary(e => e.Key, e => e.Value);

//        _attackCommand = new HeroAttackCommand(_fieldHero, weaponCard, enemiesInRange);
//    }

//    public void HandleTargetSelection()
//    {
//        if (EventSystem.current.IsPointerOverGameObject()) return;
//        if (!Input.GetMouseButtonDown(0)) return;

//        var enemy = GetMouseEnemy();
//        if (enemy == null)
//        {
//            _fieldHero.HeroData.ChangeState(HeroState.Idle);
//            return;
//        }

//        if (_attackCommand.EnemysInRange.ContainsKey(enemy))
//        {
//            _attackCommand.AddTargetLimited(enemy, _attackCommand.EnemysInRange[enemy]);
//        }
//    }

//    public void Clear()
//    {
//        _attackCommand?.DisableEnemiesOutline();
//        _attackCommand?.RemoveDices();
//        _attackCommand?.RemoveTargets();
//        _attackCommand = null;
//    }

//    private EnemyObject GetMouseEnemy()
//    {
//        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, Constants.obstacleMask) && hit.collider.CompareTag("Enemy"))
//        {
//            return hit.collider.GetComponentInParent<EnemyObject>();
//        }
//        return null;
//    }

//    private Dictionary<EnemyObject, int> EnemiesInRange(int range)
//    {
//        var objectFinder = new ObjectFinder(_fieldHero.currentCell, range);
//        return objectFinder.SearchObjectsInRage<EnemyObject>();
//    }
//}
