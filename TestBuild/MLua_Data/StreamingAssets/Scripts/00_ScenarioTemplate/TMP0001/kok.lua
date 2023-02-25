local UE = CS.UnityEngine
local LFO = require('functionsOBJ')
local LF1 = require('functionsGRP'); local LFG = LF1(Group)

UE.Debug.Log("kok is working")

local Nagn = Members.Count
local anims = {}

function start()
for i=0,Nagn - 1 do
		anims[i]=
		LFG.setPos(i,UE.Vector3(4+i,0,0))
	end
end