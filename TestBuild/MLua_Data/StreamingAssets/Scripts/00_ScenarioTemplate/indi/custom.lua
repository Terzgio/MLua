local UE = CS.UnityEngine
local LFO = require('functionsOBJ')
local RM = require('functionsROOM');

local group
local nums=3
local item

function setUp()
RM.addEmptyGroup(Room,'plane')
group = RM.addGroupMember(Room,'plane', 'models/main','Plane')


group = RM.addEmptyGroup(Room,'redspheres')
	for i=0,nums-1 do
		RM.addGroupMember(Room,'redspheres', 'models/main','RedSphere')
	end

group = RM.addEmptyGroup(Room,'bluespheres')
	for i=0,nums-1 do
		RM.addGroupMember(Room,'bluespheres', 'models/main','BlueSphere')
	end




RM.addEmptyGroup(Room,'Containers')
RM.addGroupMember(Room,'Containers', 'models/main','BlueContainer')
RM.addGroupMember(Room,'Containers', 'models/main','RedContainer')

end

