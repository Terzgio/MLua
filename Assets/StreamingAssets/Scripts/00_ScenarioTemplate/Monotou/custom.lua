local UE = CS.UnityEngine
local LFO = require('functionsOBJ')
local RM = require('functionsROOM');

function setUp()
RM.addEmptyGroup(Room,'plane')
RM.addGroupMember(Room,'plane', 'models/main','PlaneY')
end

