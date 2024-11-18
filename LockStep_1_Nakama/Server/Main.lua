local nk = require("nakama")
require("ForOut.Functions")



local GlobalUtils = require("ForOut.GlobalUtils")



-- 一上来就清空 match 存档:
GlobalUtils.StorageDeleteAllMatchs()



nk.logger_debug("-some--------------KOKO--: debug --------------")
nk.logger_info("-some--------------KOKO--: info --------------")
nk.logger_warn("-some--------------KOKO--: warn --------------")
nk.logger_error("-some--------------KOKO--: error --------------")


-- ============================ test: --> ============================





-- ============================ test: <-- ============================





--[[
-- the name must be the same as the match handler file (e.g. MatchHandler.lua)
nk.register_matchmaker_matched(function(context, matched_users)
    nk.logger_error("register_matchmaker_matched")

    local match_id, err = nk.match_create("TprMatchHandler", { invited = matched_users })
    return match_id
end)
]]


 
-- ================= RPC 示范 ======================

---@param context table @ 提供了关于当前 RPC 调用的上下文信息。包含了多个重要字段，可助于获取有关请求的详细信息
---@param payload table @ json data, c# 端的输入值 
local function _RpcDemoFunc(context, payload)
    -- Run some code.
    nk.logger_warn("=====  RpcFuncDemo ======")

    -- context:
    nk.logger_warn(" user_id: " .. tostring(context.user_id)) -- string
    nk.logger_warn(" session_id: " .. tostring(context.session_id)) -- string
    nk.logger_warn(" rpc_id: " .. tostring(context.rpc_id)) -- string
    nk.logger_warn(" match_id: " .. tostring(context.match_id)) -- string
    nk.logger_warn(" socket_id: " .. tostring(context.socket_id)) -- string

    -- table: 
    -- context.metadata

    -- table:
    --[[
        context.http.method
        context.http.headers
        context.http.body
        context.http.query
        context.http.path
        context.http.remote_addr
    ]]
    -- context.http

    -- payload 是一个 JSON 字符串，包含传入的参数  
    local params = nk.json_decode(payload)  

    nk.logger_warn("=== params ===")
    for k,v in pairs(params) do 
        nk.logger_warn("---: k:" .. tostring(k) .. "; v:" .. tostring(v))
    end 

    --- 原则上可以返回任意值
    return "this is ret value from server"
end
nk.register_rpc(_RpcDemoFunc, "RpcDemo")




-- ================= RPC storage 示范 ======================

---@param context table @ 提供了关于当前 RPC 调用的上下文信息。包含了多个重要字段，可助于获取有关请求的详细信息
---@param payload table @ json data, c# 端的输入值 
local function _RpcStorageWriteFunc(context, payload)

    -- payload 是一个 JSON 字符串，包含传入的参数  
    local params = nk.json_decode(payload)  

    -- ====== storage =====
    local new_objects = {
        { collection = "collection_1", key = "save1", user_id = nil, value = {v = params.val}, permission_read = 1, permission_write = 1 }
    }
    nk.storage_write(new_objects)

    nk.logger_warn("=====  _RpcStorageWriteFunc Done ======")

    --- 原则上可以返回任意值
    return "Store success !"
end
nk.register_rpc(_RpcStorageWriteFunc, "RpcStorageWrite")


-- ===
local function _RpcStorageReadFunc(context, payload)
    
    -- ====== storage =====
    local object_ids = {
        { collection = "collection_1", key = "save1", user_id = nil },
    }
    local objects = nk.storage_read(object_ids)
    assert( #objects == 1 )
    --for _, r in ipairs(objects) do
    --local message = string.format("read: %q, write: %q, value: %q", r.permission_read, r.permission_write, r.value)
    nk.logger_warn("=====  _RpcStorageReadFunc Done ======")
    --end

    --- 原则上可以返回任意值
    return objects[1].value.v
end
nk.register_rpc(_RpcStorageReadFunc, "RpcStorageRead")






-- ================= RPC match create ======================

---@param payload_ string @ 是一个 JSON 字符串，包含传入的参数  
---@return string | nil
local function _RpcCreateOrFindMatchFunc(context, payload_)
    nk.logger_warn("=====  _RpcCreateOrFindMatchFunc ======")

    local params = nk.json_decode(payload_)  
    local matchName = tostring(params.matchName)
    if matchName == nil then 
        nk.logger_error( "params matchName is nil" )
        return nil
    end 

    -- try Read Storage:
    local matchId = GlobalUtils.StorageReadMatch(matchName) -- maybe nil
    if matchId ~= nil then 
        -- 有这个 match:
        nk.logger_warn("== find existed match, matchId = " .. tostring(matchId) .. "; matchName = " .. tostring(matchName) )
    else 
        -- 无这个 match
        local MatchHandlerMode = "ForOut.MatchHandlerTest_1"
        -- 参数 params 只能传入简单的数据;
        matchId = nk.match_create(MatchHandlerMode, {
            matchName = matchName,
        })
        GlobalUtils.StorageWriteMatch( matchName, matchId )
        nk.logger_warn("== create new match; matchId = " .. tostring(matchId) .. "; matchName = " .. tostring(matchName) )
    end 
    --- 原则上可以返回任意值
    return matchId
end
nk.register_rpc(_RpcCreateOrFindMatchFunc, "RpcCreateOrFindMatch")





-- ================= Rpc Send InputCode ======================

---@param payload_ string @ 是一个 JSON 字符串，包含传入的参数  
---@return string | nil
local function _RpcSendInputCodeFunc(context, payload_)
    --nk.logger_warn("=====  _RpcSendInputCodeFunc ======")

    local params = nk.json_decode(payload_)  
    local inputCode = params.inputCode
    ---
    GlobalUtils.StorageWrite_InputCode( context.user_id, inputCode )
    -- --- 原则上可以返回任意值
    return "true"
end
nk.register_rpc(_RpcSendInputCodeFunc, "RpcSendInputCode")








