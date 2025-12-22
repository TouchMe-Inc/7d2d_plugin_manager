using HarmonyLib;
using PluginManager.Api.Capabilities.Implementations.Events.GameEvents;
using PluginManager.Api.Hooks;

namespace PluginManager.Core.Patches;


[HarmonyPatch(typeof(NetPackageDamageEntity), nameof(NetPackageDamageEntity.ProcessPackage))]
public static class DamagePatch
{
    static bool Prefix(NetPackageDamageEntity __instance, World _world, GameManager _callbacks)
    {
        if (_world == null)
        {
            return false;
        }
        
        var entityDamageEvent = new EntityDamageEvent(__instance.entityId, __instance.attackerEntityId, __instance.strength);
        var result = ModContext.EventRunner.Publish(entityDamageEvent, HookMode.Pre);
        
        if (_world.GetPrimaryPlayer() != null &&
            _world.GetPrimaryPlayer().entityId == __instance.entityId &&
            (__instance.damageTyp == EnumDamageTypes.Falling ||
             (__instance.damageSrc == EnumDamageSource.External &&
              (__instance.damageTyp == EnumDamageTypes.Piercing || __instance.damageTyp == EnumDamageTypes.BarbedWire) &&
              __instance.attackerEntityId == -1)))
            return false;

        Entity entity = _world.GetEntity(__instance.entityId);
        if (entity == null)
            return false;

        DamageSource damageSource = new DamageSourceEntity(
            __instance.damageSrc,
            __instance.damageTyp,
            __instance.attackerEntityId,
            __instance.dirV,
            __instance.hitTransformName,
            __instance.hitTransformPosition,
            __instance.uvHit
        );
        damageSource.SetIgnoreConsecutiveDamages(__instance.bIgnoreConsecutiveDamages);
        damageSource.DamageMultiplier = __instance.damageMultiplier;
        damageSource.BonusDamageType = (EnumDamageBonusType)__instance.bonusDamageType;
        damageSource.AttackingItem = __instance.attackingItem;
        damageSource.BlockPosition = __instance.blockPos;

        DamageResponse damageResponse = new DamageResponse
        {
            Strength = (int)__instance.strength,
            ModStrength = 0,
            MovementState = __instance.movementState,
            HitDirection = (Utils.EnumHitDirection)__instance.hitDirection,
            HitBodyPart = (EnumBodyPartHit)__instance.hitBodyPart,
            PainHit = __instance.bPainHit,
            Fatal = __instance.bFatal,
            Critical = __instance.bCritical,
            Random = __instance.random,
            Source = damageSource,
            CrippleLegs = __instance.bCrippleLegs,
            Dismember = __instance.bDismember,
            TurnIntoCrawler = __instance.bTurnIntoCrawler,
            Stun = (EnumEntityStunType)__instance.StunType,
            StunDuration = __instance.StunDuration,
            ArmorSlot = __instance.ArmorSlot,
            ArmorSlotGroup = __instance.ArmorSlotGroup,
            ArmorDamage = __instance.ArmorDamage
        };

        if (__instance.bFromBuff)
            damageResponse.Source.BuffClass = new BuffClass();

        entity.FireAttackedEvents(damageResponse);
        entity.ProcessDamageResponse(damageResponse);

        if (result != HookResult.Stop)
        {
            ModContext.EventRunner.Publish(entityDamageEvent, HookMode.Post);
        }
        
        return false;
    }
}
