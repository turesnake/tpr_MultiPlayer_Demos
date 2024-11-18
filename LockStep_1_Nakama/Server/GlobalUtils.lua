local nk = require("nakama")
require("ForOut.Functions")


--[[
    一些 公共函数
]]
local GlobalUtils = {}  


-- ============================== MatchMaps =========================================
local CollectionName_Match = "MatchMaps"


-- 每次 server 重启的时候, 删除所有 MatchMaps 中的存储:
function GlobalUtils.StorageDeleteAllMatchs() 
    local records = nk.storage_list( nil, CollectionName_Match, 100, "")

    local needToDelete = {}
    for _,r in ipairs(records) do        
        nk.logger_warn(" -- Find Match: k:" .. tostring(r.key) .. ", value:" .. tostring(r.value.match_id) )
        table.insert( needToDelete, { collection = CollectionName_Match, key = r.key, user_id = nil } )
    end
    nk.storage_delete(needToDelete)
end



function GlobalUtils.StorageWriteMatch( matchName_, matchId_ ) 
    nk.storage_write({
        { collection = CollectionName_Match, key = matchName_, user_id = nil, value = {match_id = matchId_}, permission_read = 1, permission_write = 1 }
    })
end


---@return string | nil @ match_id
function GlobalUtils.StorageReadMatch( matchName_ ) 
    if isString(matchName_) == false or #matchName_ == 0 then 
        nk.logger_error("param: matchName_ error: " .. tostring(matchName_))
        return nil
    end 
    ---
    local matchId = nil
    local readRets = nk.storage_read({
        { collection = CollectionName_Match, key = matchName_, user_id = nil }
    })

    if isTable(readRets) and #readRets==1 and readRets[1] ~= nil and readRets[1].value ~= nil and readRets[1].value.match_id ~= nil then 
        nk.logger_warn( "Success Get match_id" )
        matchId = readRets[1].value.match_id
    end 
    ---
    return matchId
end

-- ======================================== 

function GlobalUtils.StorageWrite_InputCode( user_id_, inputCode_ ) 
    nk.storage_write({
        { collection = "RoleMaps", key = "inputCode", user_id = user_id_, value = {code = inputCode_}, permission_read = 1, permission_write = 1 }
    })
end


---@return table @ inputCode[]
function GlobalUtils.StorageRead_InputCode( user_ids_ ) 
    --if istable(user_ids_) == false or #user_ids_ == 0 then 
    if  #user_ids_ == 0 then 
        nk.logger_error("param: user_ids_ error")
        return {}
    end 
    ---
    local params = {}
    for i=1, #user_ids_ do 
        table.insert(params, { collection = "RoleMaps", key = "inputCode", user_id = user_ids_[i] } )
    end 
    ---
    local retInputCodes = {}
    local readRets = nk.storage_read(params)

    if isTable(readRets) and (#readRets==#user_ids_) then 
        for i=1, #readRets do 
            table.insert( retInputCodes, readRets[i].value.code )
        end
    end 
    ---
    return retInputCodes
end





return GlobalUtils

