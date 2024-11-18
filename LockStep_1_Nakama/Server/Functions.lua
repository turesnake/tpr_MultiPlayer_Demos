
--[[
    在这个文件里直接定义一些 裸的函数, 可以被全局使用
]]


--local rapidjson = require("rapidjson")


---isNull 是否为空
---@return boolean
function isNull( data_ )
    if data_ == nil then
        return true
    end
    -- if data_ == rapidjson.null then
    --     return true
    -- end
    return false
end

---isNotNull 是否非空
---@return boolean
function isNotNull(data_)
    return isNull(data_) == false
end


---@param value_ any
---@return boolean
function isTable(value_)
    return isNotNull(value_) and type(value_) == 'table'
end


---@param value_ any
---@return boolean
function isFunction(value_)
    return isNotNull(value_) and type(value_) == 'function'
end


---@param value_ any
---@return boolean
function isTable(value_)
    return isNotNull(value_) and type(value_) == 'table'
end

---@param value_ any
---@return boolean
function isString(value_)
    return isNotNull(value_) and type(value_) == 'string'
end

---@param value_ any
---@return boolean
function isNumber(value_)
    return isNotNull(value_) and type(value_) == 'number'
end



-- ===============================

--- check integer
---@param value any
---@return integer
function checkInt(value)
    return math.floor((tonumber(value) or 0) + 0.5)
end


--- check table
---@generic T
---@param value T
---@return T
function checkTable(value)
    if type(value) ~= "table" then
        value = {}
    end
    return value
end



--- ================= table 相关扩展 =================

-- 计算哈希表长度
local function count(hashtable)
    local count = 0
    for _, _ in pairs(hashtable) do
        count = count + 1
    end
    return count
end

table.count = count


--- ================= string 相关扩展 =================

local function isEmpty(value)
    return value == nil or string.len(tostring(value)) == 0
end



string.isEmpty = isEmpty
















































