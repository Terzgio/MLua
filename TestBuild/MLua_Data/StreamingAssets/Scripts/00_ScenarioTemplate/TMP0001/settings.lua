-- aliases --
local UE = CS.UnityEngine
local VFX = require('effects')
local UT = require('utils')
local RM = require('functionsROOM');

-- variables --
local Nagn=2
local group
function applySettings()
	UE.Screen.SetResolution(1440,900, true, 0)
	UE.QualitySettings.SetQualityLevel(5)
	UE.Application.targetFrameRate = 30
	UE.QualitySettings.vSyncCount = 0
	UE.QualitySettings.shadows = UE.ShadowQuality.All
end

function setUp()
	group = RM.addEmptyGroup(Room,'dancers','group.lua')
	for i=0,Nagn-1 do
		RM.addGroupMember(Room,'dancers', 'models/main','NeoMan')
	end
	group = RM.addEmptyGroup(Room, 'bluecont', 'kok.lua')
	RM.addGroupMember(Room, 'bluecont', 'models/main', 'BlueContainer')
	
	RM.runGroupScript(Room,'dancers')
	RM.runGroupScript(Room,'bluecont')
	
	RM.addCamera(Room)
	setMusic('PercStudio/Silamalon.ogg')
	VFX.clearAllGlobalEffects()
end

function update()
    VFX.checkGlobalEffectInputs()
	UT.listenToGenericShortcuts()
end

-- Everytime the script is reloaded the settings are applied.
applySettings();
