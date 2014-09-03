﻿using System;

public enum ViUnitSocietyMask
{
	SELF = 0X01,
	FRIEND = 0X02,
	ENEMY = 0X04,
	NEUTRAL = 0X08,
	TEAM = 0X10,
	GROUND = 0X20,
	SELF_FRONT_GROUND = 0X40,
	SELF_FRONT_OBJECT = 0X80,
}
public enum ViMoveDirection
{
	SELF,
	SELF_TARGET,
	TARGET,
	TARGET_SELF,
}

public enum  ViGameMessageIdx
{
	NONE,
	SPELL_SUCCESS,
	SPELL_FAILED_NO_INFO,
	SPELL_FAILED_BUSY,
	SPELL_FAILED_CASTER_MOVE,
	SPELL_FAILED_CASTER_AURA,
	SPELL_FAILED_CASTER_ACTION,
	SPELL_FAILED_CASTER_POWER,///(Attr)

	SPELL_FAILED_TARGET_ERROR,
	SPELL_FAILED_TARGET_IS_FREE,
	SPELL_FAILED_TARGET_NOT_IN_FRONT,
	SPELL_FAILED_TARGET_OUT_RANGE,

	ACTION_REQ_STATE_NOT_MATCH,
	ACTION_NOT_STATE_NOT_MATCH,
	MOVE_REQ_STATE_NOT_MATCH,
	MOVE_NOT_STATE_NOT_MATCH,
	AURA_REQ_STATE_NOT_MATCH,
	AURA_NOT_STATE_NOT_MATCH,

	TOTAL,
}

public enum ViAccumulateTimeType
{
	SELF,
	WORLD,
}