
这是一个尚未完工的版本; 

# 目前实现的:
-- client 及时将自己的操作 inputcode 上传给 server;
-- server 端维持一个 10 fps 的 loop; 每一循环都把收集到的最新的 inputcodes 广播给 match (room) 中的所有 clients;
-- client 接收到 server 端的广播后, 立刻更新本地所有 roles 的 inputcodes;
-- client 自己维护一个 60 fps 的运动层loop, 在里面每一个 role使用自己记录的 inputcode, 去驱动自己的运动;

# 存在的问题:
-- 支持 单个角色登录 match;  多个角色登录 match 尚未验证, 可能存在细微 bug 要修;
-- 这个方案存在严重的 同步问题, 随着时间的推移, 每个 client 上的表现差异会越来越大


# 优点:
-- 学会了使用 nakama 的 rpc, match handler, storage 的应用;



# ================================== #
#           Client
# ================================== #


# ================================== #
#           Server
# ================================== #
部署在 Nakama 端;














