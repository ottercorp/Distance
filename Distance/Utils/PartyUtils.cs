using FFXIVClientStructs.FFXIV.Client.Game.Group;

namespace Distance;

internal static unsafe class PartyUtils
{
	internal static bool ObjectIsPartyMember( uint entityID )
	{
		if( entityID is 0 or 0xE0000000 ) return false;
		if( Service.PartyList.Length < 1 ) return false;
		foreach( var member in Service.PartyList ) if( member?.EntityId == entityID ) return true;
		return false;
	}

	internal static bool ObjectIsAllianceMember( uint entityID )
	{
		if( entityID is 0 or 0xE0000000 ) return false;
		if( GroupManager.Instance() == null ) return false;
		//if( !GroupManager.Instance()->IsAlliance ) return false;	//***** TODO: IsAlliance always returns false; why?
		if( GroupManager.Instance()->MainGroup.IsEntityIdInParty( entityID ) ) return false;
		return GroupManager.Instance()->MainGroup.IsEntityIdInAlliance( entityID );
	}
}
