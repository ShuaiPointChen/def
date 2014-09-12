def
===

Distributed Entity Framework

* 引擎介绍

  DEF（Distributed Entity Framework）是基于Unity3D扩充的纯C#服务端引擎，借鉴了bigworld，pomelo的设计思路，目标是通过Unity3D+DEF形成一套mmorpg手机游戏整体解决方案。

  与pomelo的差异：

  a. 统一的C#语言使得业务层可以构建CS公共业务库；

  b. 使用类Unity3D的实体组件系统写业务逻辑，逻辑程序员可以不分服务端客户端，一个人搞定整个任务系统，道具系统等模块。

* 引擎约束

  NodeType不可以超过255个;

  Node实例总数不可以超过ushort个;

  每个Node上运行时Entity实例不可以超过uint个;

  EntityType是string类型，应用层最好使用枚举转string方式提供该参数，减少出错可能性;

  建议使用文档描述项目中Entity的层次关系和生命周期时序。
