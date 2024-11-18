local nk = require("nakama")
require("ForOut.Functions")


local GlobalUtils = require("ForOut.GlobalUtils")




local MatchHandlerTest_1 = {}


--[[
    这些 match handle 函数似乎是运行在 别的 环境或者线程中, 导致它无法共享 rpc 中的 singleton 数据;



]]

-- nk.match_create() 调用的 handle
function MatchHandlerTest_1.match_init(context, params)
    nk.logger_warn("match_init, name = " .. tostring(params.matchName))

    -- ======



    -- ======

    -- 本 match instance 的 fields: 下面每个 handler函数中都能访问到;
    local state = {  
        matchName = params.matchName, -- match_create() 传入
        ---
        presences = {}, -- 在线玩家账号; list
        empty_ticks = 0, -- 当没有玩家在线时, 本 match 关闭前的 等待计数器
        tick_rate = 60, -- 每秒调用 match_loop() 几次  
        ---
        lastTime_BroadcastMsg = os.clock(), 
        interval_BroadcastMsg = 0.1, -- 多久主动 广播一次消息 
        --- 
        lastTime_Logic = os.clock(), 
        interval_Logic = 0.1 

    }  
    local label = ""
    return state, state.tick_rate, label
end
  
  

-- 不做筛查, 一律允许:
---@param metadata table @ client socket.JoinMatchAsync() 的参数
function MatchHandlerTest_1.match_join_attempt(context, dispatcher, tick, state_, presence, metadata)
    nk.logger_warn("match_join_attempt")

    -- !!! 过滤

    local acceptuser = true
    return state_, acceptuser
end





function MatchHandlerTest_1.match_join(context, dispatcher, tick, state_, presences_)
    nk.logger_warn("match_join")
    for _, p in ipairs(presences_) do
        nk.logger_warn("-- join user_id: " .. tostring(p.user_id))
        state_.presences[p.user_id] = p 
        --table.insert( state_.presences, p )

        --
        GlobalUtils.StorageWrite_InputCode( p.user_id, 0 ) 
    end
    return state_
end


function MatchHandlerTest_1.match_leave(context, dispatcher, tick, state_, presences_)
    nk.logger_warn("match_leave")
    for _, p in ipairs(presences_) do
        nk.logger_warn("-- leave user_id: " .. tostring(p.user_id))
        state_.presences[p.user_id] = nil

    end
    return state_
end



-- ==============================================

-- 这个帧 间距 并不准时...

-- 每秒调用的次数 由 match_init() 中指定:
---@return table | nil @  返回 nil 是关闭 match 的唯一方法
function MatchHandlerTest_1.match_loop(context, dispatcher, tick, state_, messages)  
    local currentTime = os.clock()  
    -- ======================== Logic ======================== 
    -- if currentTime - state_.lastTime_Logic >= state_.interval_Logic then  
    --     MatchHandlerTest_1.Loop_Logic( context, dispatcher, tick, state_, messages, currentTime )
    --     ---
    --     state_.lastTime_Logic = currentTime  
    -- end


    -- Get the count of presences in the match
    local totalPresences = 0
    for k, v in pairs(state_.presences) do
        totalPresences = totalPresences + 1
    end
    -- If we have no presences in the match according to the match state, increment the empty ticks count
    if totalPresences == 0 then
        state_.empty_ticks = state_.empty_ticks + 1
    else 
        state_.empty_ticks = 0
    end
    -- If the match has been empty for more than 100 ticks, end the match by returning nil
    if state_.empty_ticks > 100 then
        -- !!!! match 被关闭的位置:
        nk.logger_warn("match_loop: ret nil")

        local matchName = state_.matchName
        local matchId = GlobalUtils.StorageReadMatch(matchName) -- maybe nil
        if matchId ~= nil then 
            -- 找到了
            GlobalUtils.StorageWriteMatch( matchName, nil )
            nk.logger_warn( "Delete match; name: " .. tostring(matchName) )
        else 
            -- 没找到
            nk.logger_error( "Not Find match in matchMaps; name: " .. tostring(matchName) )
        end 
        return nil
    end

    -- ======================== Logic ======================== 
    if totalPresences == 0 then
        return
    end
    if currentTime - state_.lastTime_BroadcastMsg >= state_.interval_BroadcastMsg then  
        MatchHandlerTest_1.Loop_BroadcastMsg(context, dispatcher, tick, state_, messages, currentTime)
        ---
        state_.lastTime_BroadcastMsg = currentTime  
    end  
    ---
    return state_  
end  



function MatchHandlerTest_1.Loop_Logic(context, dispatcher, tick, state_, messages, currentTime)  
    -- Get the count of presences in the match
    local totalPresences = 0
    for k, v in pairs(state_.presences) do
        totalPresences = totalPresences + 1
    end
    -- If we have no presences in the match according to the match state, increment the empty ticks count
    if totalPresences == 0 then
        state_.empty_ticks = state_.empty_ticks + 1
    else 
        state_.empty_ticks = 0
    end
    -- If the match has been empty for more than 100 ticks, end the match by returning nil
    if state_.empty_ticks > 10 then
        -- !!!! match 被关闭的位置:
        nk.logger_warn("match_loop: ret nil")

        local matchName = state_.matchName
        local matchId = GlobalUtils.StorageReadMatch(matchName) -- maybe nil
        if matchId ~= nil then 
            -- 找到了
            GlobalUtils.StorageWriteMatch( matchName, nil )
            nk.logger_warn( "Delete match; name: " .. tostring(matchName) )
        else 
            -- 没找到
            nk.logger_error( "Not Find match in matchMaps; name: " .. tostring(matchName) )
        end 
        return nil
    end
end



function MatchHandlerTest_1.Loop_BroadcastMsg(context, dispatcher, tick, state_, messages, currentTime)  

    --[[
    local opcode = 1234 -- Numeric message op code.
    --local message = { ["os_time"] = tostring(currentTime) }
    local encoded = nk.json_encode(message) -- Data to be sent to the provided presences, or nil.
    local presences = nil -- send to all.
    local sender = nil -- used if a message should come from a specific user.
    local err = dispatcher.broadcast_message(opcode, encoded, presences, sender)
    ]]
    ----

    local user_ids = {} -- user_id[]
    for _,p in pairs(state_.presences) do 
        table.insert( user_ids, p.user_id )
    end
    local inputCodes = GlobalUtils.StorageRead_InputCode(user_ids) -- inputCode[] 


    local opcode = 11 -- Numeric message op code.
    local msg = { ["user_ids"] = user_ids, ["inputCodes"] = inputCodes }
    local encoded = nk.json_encode(msg)
    local presences = nil -- send to all.
    local sender = nil -- used if a message should come from a specific user.
    local err = dispatcher.broadcast_message(opcode, encoded, presences, sender)



    -- if not success then  
    --     nk.logger_warn("match_loop - broadcast_message error: " .. err)  
    -- else  
    --     nk.logger_info("match_loop - broadcast_message successful")  
    -- end 

end

-- ==============================================


-- 仅当 nakama server 被 graceful shutdown 时, 本函数才被调用;
-- 适合向 clients 广播个信息来通知 server 被关闭了;
function MatchHandlerTest_1.match_terminate(context, dispatcher, tick, state_, grace_seconds)
    nk.logger_warn("match_terminate")

    local message = "Server shutting down in " .. grace_seconds .. " seconds"
    dispatcher.broadcast_message(2, message)
    return nil
end



-- 暂时没想到怎么用它:   -- !!! 也许和 signal 机制有关;
-- Called when the match handler receives a runtime signal. 
-- Match signals allow the match handler to be sent a reservation signal to mark a user ID or session ID into the match state ahead of their join attempt 
-- and eventual join flow. 
-- This is useful to apply reservations to a matchmaking system with Nakama's matchmaker or match listings APIs.
function MatchHandlerTest_1.match_signal(context, dispatcher, tick, state_, data)
    nk.logger_warn("match_signal")
    return state_, "signal received: " .. data
end



-- function MatchHandlerTest_1.on_event(event)  
--     --print("Event received: " .. event.data)  
--     -- 处理事件数据  
--     nk.logger_warn( "~~ event = " .. tostring(event) )
-- end



return MatchHandlerTest_1

