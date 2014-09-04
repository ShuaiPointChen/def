using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace Zk
{
    public enum ZooLogLevel
    {
        ZOO_LOG_LEVEL_ERROR = 1,
        ZOO_LOG_LEVEL_WARN = 2,
        ZOO_LOG_LEVEL_INFO = 3,
        ZOO_LOG_LEVEL_DEBUG = 4
    };

    public enum ZOO_EVENT
    {
        /* zookeeper event type constants */
        CREATED_EVENT_DEF = 1,
        DELETED_EVENT_DEF = 2,
        CHANGED_EVENT_DEF = 3,
        CHILD_EVENT_DEF = 4,
        SESSION_EVENT_DEF = -1,
        NOTWATCHING_EVENT_DEF = -2,
    }

    public enum ZOO_OPE
    {
        /*ZooKeeper operation type*/
        ZOO_NOTIFY_OP = 0,
        ZOO_CREATE_OP = 1,
        ZOO_DELETE_OP = 2,
        ZOO_EXISTS_OP = 3,
        ZOO_GETDATA_OP = 4,
        ZOO_SETDATA_OP = 5,
        ZOO_GETACL_OP = 6,
        ZOO_SETACL_OP = 7,
        ZOO_GETCHILDREN_OP = 8,
        ZOO_SYNC_OP = 9,
        ZOO_PING_OP = 11,
        ZOO_GETCHILDREN2_OP = 12,
        ZOO_CHECK_OP = 13,
        ZOO_MULTI_OP = 14,
        ZOO_CLOSE_OP = -11,
        ZOO_SETAUTH_OP = 100,
        ZOO_SETWATCHES_OP = 101,
    }

    enum ZOO_STATE
    {
        /* zookeeper state constants */
        EXPIRED_SESSION_STATE_DEF = -112,
        AUTH_FAILED_STATE_DEF = -113,
        CONNECTING_STATE_DEF = 1,
        ASSOCIATING_STATE_DEF = 2,
        CONNECTED_STATE_DEF = 3,
        NOTCONNECTED_STATE_DEF = 999,
    }

    public enum ZOO_ERRORS
    {
        ZOK = 0, /*!< Everything is OK */
        /** System and server-side errors.
         * This is never thrown by the server, it shouldn't be used other than
         * to indicate a range. Specifically error codes greater than this
         * value, but lesser than {@link #ZAPIERROR}, are system errors. */
        ZSYSTEMERROR = -1,
        ZRUNTIMEINCONSISTENCY = -2, /*!< A runtime inconsistency was found */
        ZDATAINCONSISTENCY = -3, /*!< A data inconsistency was found */
        ZCONNECTIONLOSS = -4, /*!< Connection to the server has been lost */
        ZMARSHALLINGERROR = -5, /*!< Error while marshalling or unmarshalling data */
        ZUNIMPLEMENTED = -6, /*!< Operation is unimplemented */
        ZOPERATIONTIMEOUT = -7, /*!< Operation timeout */
        ZBADARGUMENTS = -8, /*!< Invalid arguments */
        ZINVALIDSTATE = -9, /*!< Invliad zhandle state */
        /** API errors.
         * This is never thrown by the server, it shouldn't be used other than
         * to indicate a range. Specifically error codes greater than this
         * value are API errors (while values less than this indicate a 
         * {@link #ZSYSTEMERROR}).
         */
        ZAPIERROR = -100,
        ZNONODE = -101, /*!< Node does not exist */
        ZNOAUTH = -102, /*!< Not authenticated */
        ZBADVERSION = -103, /*!< Version conflict */
        ZNOCHILDRENFOREPHEMERALS = -108, /*!< Ephemeral nodes may not have children */
        ZNODEEXISTS = -110, /*!< The node already exists */
        ZNOTEMPTY = -111, /*!< The node has children */
        ZSESSIONEXPIRED = -112, /*!< The session has been expired by the server */
        ZINVALIDCALLBACK = -113, /*!< Invalid callback specified */
        ZINVALIDACL = -114, /*!< Invalid ACL specified */
        ZAUTHFAILED = -115, /*!< Client authentication failed */
        ZCLOSING = -116, /*!< ZooKeeper is closing */
        ZNOTHING = -117, /*!< (not error) no server responses to process */
        ZSESSIONMOVED = -118 /*!<session moved to another server, so operation is ignored */
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct String_vector
    {
        public int count;
        public IntPtr data;
    };

    public class WatchedEvent
    {
        //---------------------------------------------------------------------
        private readonly int state;
        private readonly int type;
        private readonly string path;

        //---------------------------------------------------------------------
        public WatchedEvent(int state, int type, string path)
        {
            this.state = state;
            this.type = type;
            this.path = path;
        }

        //---------------------------------------------------------------------
        public int State
        {
            get { return state; }
        }

        //---------------------------------------------------------------------
        public int Type
        {
            get { return type; }
        }

        //---------------------------------------------------------------------
        public string Path
        {
            get { return path; }
        }

        //---------------------------------------------------------------------
        public override string ToString()
        {
            return "WatchedEvent type:" + (ZOO_EVENT)type
                + " state:" + (ZOO_STATE)state + " path:" + path;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ZK_CONST
    {
        //---------------------------------------------------------------------
        public readonly static int ZOO_EPHEMERAL = 1 << 0; //临时节点，断线自动删除.
        public readonly static int ZOO_SEQUENCE = 1 << 1;  //序列号的节点.
        public static readonly int ADD_WATCH = 1;
        public static readonly int NOT_WATCH = 0;
        public readonly static int ZOO_STR_BUF_LEN = 16 * 1024;  //同步接口公用字符串缓存大小(16k).
        private static IntPtr _world = IntPtr.Zero;
        private static IntPtr _anyone = IntPtr.Zero;
        private static IntPtr _auth = IntPtr.Zero;
        private static IntPtr _null = IntPtr.Zero;
        private static IntPtr PTR_OPEN_ACL_UNSAFE_ACL = IntPtr.Zero;
        private static IntPtr PTR_READ_ACL_UNSAFE_ACL = IntPtr.Zero;
        private static IntPtr PTR_CREATOR_ALL_ACL_ACL = IntPtr.Zero;
        public static IntPtr ZOO_OPEN_ACL_UNSAFE = IntPtr.Zero;
        public static IntPtr ZOO_READ_ACL_UNSAFE = IntPtr.Zero;
        public static IntPtr ZOO_CREATOR_ALL_ACL = IntPtr.Zero;
        private static ACL _OPEN_ACL_UNSAFE_ACL;
        private static ACL _READ_ACL_UNSAFE_ACL;
        private static ACL _CREATOR_ALL_ACL_ACL;
        private static ACL_vector VEC_OPEN;
        private static ACL_vector VEC_READ;
        private static ACL_vector VEC_CREA;
        // 用于同步函数传参数.
        public static IntPtr STR_BUFFER = IntPtr.Zero;
        public static IntPtr STRS_BUFFER = IntPtr.Zero;
        public static IntPtr INT_BUFFER = IntPtr.Zero;

        //---------------------------------------------------------------------
        public static void Init()
        {
            _world = Marshal.StringToHGlobalAnsi("world\0");
            _anyone = Marshal.StringToHGlobalAnsi("anyone\0");
            _auth = Marshal.StringToHGlobalAnsi("auth\0");
            _null = Marshal.StringToHGlobalAnsi("\0");

            PTR_OPEN_ACL_UNSAFE_ACL = Marshal.AllocHGlobal(Marshal.SizeOf((typeof(ACL))));
            PTR_READ_ACL_UNSAFE_ACL = Marshal.AllocHGlobal(Marshal.SizeOf((typeof(ACL))));
            PTR_CREATOR_ALL_ACL_ACL = Marshal.AllocHGlobal(Marshal.SizeOf((typeof(ACL))));

            ZOO_OPEN_ACL_UNSAFE = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ACL_vector)));
            ZOO_READ_ACL_UNSAFE = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ACL_vector)));
            ZOO_CREATOR_ALL_ACL = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ACL_vector)));

            _OPEN_ACL_UNSAFE_ACL.Perms = 0x1f;
            _OPEN_ACL_UNSAFE_ACL.Scheme = _world;
            _OPEN_ACL_UNSAFE_ACL.Id = _anyone;

            _READ_ACL_UNSAFE_ACL.Perms = 0x01;
            _READ_ACL_UNSAFE_ACL.Scheme = _world;
            _READ_ACL_UNSAFE_ACL.Id = _anyone;

            _CREATOR_ALL_ACL_ACL.Perms = 0x1f;
            _CREATOR_ALL_ACL_ACL.Scheme = _auth;
            _CREATOR_ALL_ACL_ACL.Id = _null;

            Marshal.StructureToPtr(_OPEN_ACL_UNSAFE_ACL, PTR_OPEN_ACL_UNSAFE_ACL, false);
            Marshal.StructureToPtr(_READ_ACL_UNSAFE_ACL, PTR_READ_ACL_UNSAFE_ACL, false);
            Marshal.StructureToPtr(_CREATOR_ALL_ACL_ACL, PTR_CREATOR_ALL_ACL_ACL, false);

            VEC_OPEN.count = 1;
            VEC_OPEN.data = PTR_OPEN_ACL_UNSAFE_ACL;

            VEC_READ.count = 1;
            VEC_READ.data = PTR_OPEN_ACL_UNSAFE_ACL;

            VEC_CREA.count = 1;
            VEC_CREA.data = PTR_OPEN_ACL_UNSAFE_ACL;

            Marshal.StructureToPtr(VEC_OPEN, ZOO_OPEN_ACL_UNSAFE, false);
            Marshal.StructureToPtr(VEC_READ, ZOO_READ_ACL_UNSAFE, false);
            Marshal.StructureToPtr(VEC_CREA, ZOO_CREATOR_ALL_ACL, false);

            STR_BUFFER = Marshal.AllocHGlobal(ZOO_STR_BUF_LEN);
            STRS_BUFFER = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(String_vector)));
            INT_BUFFER = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
            Marshal.WriteInt32(INT_BUFFER, ZOO_STR_BUF_LEN);
        }

        //---------------------------------------------------------------------
        public static void Release()
        {
            if (_world != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_world);
                Marshal.FreeHGlobal(_anyone);
                Marshal.FreeHGlobal(_auth);
                Marshal.FreeHGlobal(_null);

                Marshal.FreeHGlobal(ZOO_OPEN_ACL_UNSAFE);
                Marshal.FreeHGlobal(ZOO_READ_ACL_UNSAFE);
                Marshal.FreeHGlobal(ZOO_CREATOR_ALL_ACL);

                Marshal.FreeHGlobal(PTR_OPEN_ACL_UNSAFE_ACL);
                Marshal.FreeHGlobal(PTR_READ_ACL_UNSAFE_ACL);
                Marshal.FreeHGlobal(PTR_CREATOR_ALL_ACL_ACL);

                Marshal.FreeHGlobal(STR_BUFFER);
                Marshal.FreeHGlobal(STRS_BUFFER);
                Marshal.FreeHGlobal(INT_BUFFER);
            }

            _world = IntPtr.Zero;
            _anyone = IntPtr.Zero;
            _auth = IntPtr.Zero;
            _null = IntPtr.Zero;

            ZOO_OPEN_ACL_UNSAFE = IntPtr.Zero;
            ZOO_READ_ACL_UNSAFE = IntPtr.Zero;
            ZOO_CREATOR_ALL_ACL = IntPtr.Zero;

            PTR_OPEN_ACL_UNSAFE_ACL = IntPtr.Zero;
            PTR_READ_ACL_UNSAFE_ACL = IntPtr.Zero;
            PTR_CREATOR_ALL_ACL_ACL = IntPtr.Zero;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ACL
    {
        public int Perms;
        public IntPtr Scheme;
        public IntPtr Id;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ACL_vector
    {
        public int count;
        public IntPtr data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Stat
    {
        //---------------------------------------------------------------------
        public long Czxid { get; set; }
        public long Mzxid { get; set; }
        public long Ctime { get; set; }
        public long Mtime { get; set; }
        public int Version { get; set; }
        public int Cversion { get; set; }
        public int Aversion { get; set; }
        public long EphemeralOwner { get; set; }
        public int DataLength { get; set; }
        public int NumChildren { get; set; }
        public long Pzxid { get; set; }

        //---------------------------------------------------------------------
        public int CompareTo(object peer_)
        {
            if (!(peer_ is Stat))
            {
                throw new InvalidOperationException("Comparing different types of records.");
            }
            Stat peer = (Stat)peer_;
            int ret = 0;
            ret = (Czxid == peer.Czxid) ? 0 : ((Czxid < peer.Czxid) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Mzxid == peer.Mzxid) ? 0 : ((Mzxid < peer.Mzxid) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Ctime == peer.Ctime) ? 0 : ((Ctime < peer.Ctime) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Mtime == peer.Mtime) ? 0 : ((Mtime < peer.Mtime) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Version == peer.Version) ? 0 : ((Version < peer.Version) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Cversion == peer.Cversion) ? 0 : ((Cversion < peer.Cversion) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Aversion == peer.Aversion) ? 0 : ((Aversion < peer.Aversion) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (EphemeralOwner == peer.EphemeralOwner) ? 0 : ((EphemeralOwner < peer.EphemeralOwner) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (DataLength == peer.DataLength) ? 0 : ((DataLength < peer.DataLength) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (NumChildren == peer.NumChildren) ? 0 : ((NumChildren < peer.NumChildren) ? -1 : 1);
            if (ret != 0) return ret;
            ret = (Pzxid == peer.Pzxid) ? 0 : ((Pzxid < peer.Pzxid) ? -1 : 1);
            if (ret != 0) return ret;
            return ret;
        }
    }

    public class zookeeper
    {
        //---------------------------------------------------------------------
        public static string[] getString_vector(IntPtr strings)
        {
            // 安全模式.
            String_vector str = new String_vector();
            str = (String_vector)Marshal.PtrToStructure(strings, typeof(String_vector));
            if (str.count <= 0) return null;
            string[] childs = new string[str.count];
            IntPtr[] ptr = new IntPtr[str.count];
            Marshal.Copy(str.data, ptr, 0, str.count);
            for (int index = 0; index < str.count; index++)
            {
                childs[index] = Marshal.PtrToStringAnsi(ptr[index]);
            }
            return childs;

            //unsafe
            //{
            //    String_vector* str = (String_vector*)strings.ToPointer();
            //    string[] childs = new string[str->count];
            //    IntPtr[] ptr = new IntPtr[str->count];
            //    Marshal.Copy(str->data, ptr, 0, str->count);
            //    for (int index = 0; index < str->count; index++)
            //    {
            //        childs[index] = Marshal.PtrToStringAnsi(ptr[index]);
            //    }
            //    return childs;
            //}
        }

        //---------------------------------------------------------------------
        public static string getString(IntPtr data)
        {
            if (null == data) return null;
            string temp = Marshal.PtrToStringAnsi(data);
            return temp;
        }

        //---------------------------------------------------------------------
        public static string getString2(IntPtr data, int lenght)
        {
            if (null == data) return null;
            string temp = Marshal.PtrToStringAnsi(data, lenght);
            return temp;
        }

        //---------------------------------------------------------------------
        public static string getString2(IntPtr data, IntPtr lenght)
        {
            if (null == data) return null;
            string temp = Marshal.PtrToStringAnsi(data, Marshal.ReadInt32(lenght));
            return temp;
        }
        //---------------------------------------------------------------------
        public static int getInt(IntPtr data)
        {
            if (null == data) return -1;
            return Marshal.ReadInt32(data);
        }
        //---------------------------------------------------------------------
        public static string error2str(int cd)
        {
            return Marshal.PtrToStringAnsi(zerror(cd));
        }

        //---------------------------------------------------------------------
        public static void FreeMem(IntPtr data)
        {
            Marshal.FreeHGlobal(data);
        }

        //---------------------------------------------------------------------
        public static Stat getStat(IntPtr stat)
        {
            Stat rt = new Stat();
            rt = (Stat)Marshal.PtrToStructure(stat, typeof(Stat));
            return rt;
        }

        //---------------------------------------------------------------------
        public static StructType ConverBytesToStructure<StructType>(byte[] bytesBuffer)
        {
            // 检查长度。
            if (bytesBuffer.Length != Marshal.SizeOf(typeof(StructType)))
            {
                throw new ArgumentException("bytesBuffer参数和structObject参数字节长度不一致");
            }

            IntPtr bufferHandler = Marshal.AllocHGlobal(bytesBuffer.Length);
            for (int index = 0; index < bytesBuffer.Length; index++)
            {
                Marshal.WriteByte(bufferHandler, index, bytesBuffer[index]);
            }
            StructType structObject = (StructType)Marshal.PtrToStructure(bufferHandler, typeof(StructType));
            Marshal.FreeHGlobal(bufferHandler);
            return structObject;
        }

        private static int _IncreaseId = 0 ;
        public static int generateId()
         {
           return  Interlocked.Increment(ref _IncreaseId);
         }

        //---------------------------------------------------------------------
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void watcher_fn(IntPtr zh, int type, int state, string path, IntPtr watcherCtx);

        //---------------------------------------------------------------------
        //功能：
        //创建一个句柄（handle）和一个响应（response）这个句柄的会话（session）。
        //参数：
        //host：zookeeper主机列表，用逗号间隔。
        //fn：用于监视的回调函数。
        //clientid：之前建立过连接，现在要重新连的客户端（client）ID。如果之前没有，则为0.

        [DllImport("zookeeper.dll", EntryPoint = "zookeeper_init", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zookeeper_init(string host, watcher_fn fn,
                      int recv_timeout, IntPtr clientid, string context, int flags);

        //---------------------------------------------------------------------
        [DllImport("zookeeper.dll", EntryPoint = "zoo_set_debug_level", CallingConvention = CallingConvention.Cdecl)]
        public static extern void zoo_set_debug_level(int logLvl);

        //---------------------------------------------------------------------
        // 返回 void 类型的回调函数
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void void_completion_t(int rc, IntPtr data);

        //---------------------------------------------------------------------
        // 返回 Stat 结构的回调函数
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void stat_completion_t(int rc, IntPtr stat, IntPtr data);

        //---------------------------------------------------------------------
        // 返回字符串的回调函数
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void string_completion_t(int rc, IntPtr value, IntPtr data);

        //---------------------------------------------------------------------
        // 返回数据的回调函数
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void data_completion_t(int rc, IntPtr value, int value_len, IntPtr stat, IntPtr data);

        //---------------------------------------------------------------------
        // 返回字符串列表(a list of string)的回调函数
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void strings_completion_t(int rc, IntPtr strings, IntPtr data);

        //---------------------------------------------------------------------
        // 同时返回字符串列表(a list of string)和 Stat 结构的回调函数
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void strings_stat_completion_t(int rc, IntPtr strings, IntPtr stat, IntPtr data);

        //---------------------------------------------------------------------
        //zh	  zookeeper_init() 返回的 zookeeper 句柄。
        //path	  节点路径。
        //watch	  如果非 0，则在服务器端设置监视，当节点发生变化时客户端会得到通知，即使当前指定的节点不存在也会设置监视，这样该节点被创建时，客户端也可以得到通知。
        //completion	当 zoo_aexists 请求完成时会调用该函数，该函数原型详见第三讲《回调函数》一节。同时传递给completion的 rc参数为: ZOK 操作完成；ZNONODE 节点不存在；ZNOAUTH 客户端没有权限删除节点。
        //data	  completion 函数被调用时，传递给 completion 的数据。
        //watcherCtx	用户指定的数据，将被传入到监视器回调函数中，与由 zookeeper_init() 设置的全局监视器上下文不同，该函数设置的监视器上下文只与当前的监视器相关联。

        //---------------------------------------------------------------------
        //功能：
        //创建一个同步的zookeeper节点。
        //参数：
        //zh：zookeeper的句柄，由zookeeper_init得到。
        //path：节点名称，就是一个类似于文件系统写法的路径。
        //value：欲存储到该节点的数据。如果不存储数据，则设置为NULL。
        //valuelen：欲存储的数据的长度。如果不存储数据，则设置为-1.
        //acl：初始的ACL节点，ACL不能为空。比如设置为&ZOO_OPEN_ACL_UNSAFE。（TODO）
        //flags：一般设置为0.（TODO）
        //path_buffer：将由新节点填充的路径值。可设置为NULL。（TODO）
        //path_buffer_len：path_buffer的长度。
        //返回值：ZOK，ZNONODE，ZNODEEXISTS，ZNOAUTH，ZNOCHILDRENFOREPHEMERALS，ZBADARGUMENTS，ZINVALIDSTATE，ZMARSHALLINGERROR。ZOK表示操作成功，ZNONODE表示该节点不存在，ZNODEEXISTS表示节点已经存在，ZNOAUTH表示客户端（client）无权限，ZNOCHILDRENFOREPHEMERALS表示不能够创建临时（ephemeral）节点的子节点（children），ZINVALIDSTATE表示存在非法的参数，后两者暂略（TODO）
        [DllImport("zookeeper.dll", EntryPoint = "zoo_acreate", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_acreate(IntPtr zh,
                                              string path,
                                              string value,
                                              int valuelen,
                                              IntPtr acl,
                                              int flags,
                                              string_completion_t completion,
                                              IntPtr data
                                              );

        //---------------------------------------------------------------------
        [DllImport("zookeeper.dll", EntryPoint = "zoo_adelete", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_adelete(IntPtr zh, string path, int version, void_completion_t completion, IntPtr data);

        //---------------------------------------------------------------------
        //检查节点状态 exists(两个，分别是 zoo_aexists() 和 zoo_awexists()，区别是后者可以指定单独的 watcher_fn(监视器回调函数)，
        //而前者只能用 zookeeper_init() 设置的全局监视器回调函数
        [DllImport("zookeeper.dll", EntryPoint = "zoo_aexists", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_aexists(IntPtr zh, string path, int watch,
                               stat_completion_t completion, IntPtr data);

        //---------------------------------------------------------------------
        //功能：
        //同步监视一个zookeeper节点（node）是否存在。
        //参数：
        //zh：zookeeper的句柄，由zookeeper_init得到。
        //path：节点名称，就是一个类似于文件系统写法的路径。
        //watch：设置为0，则无作用。设置为非0时，则会监听节点的改变.
        //stat：（TODO）
        //返回值：ZOK，ZNONODE，ZNOAUTH，ZBADARGUMENTS，ZINVALIDSTATE，ZMARSHALLINGERROR。ZOK表示操作成功，ZNONODE表示该节点不存在，ZNOAUTH表示客户端（client）无权限，ZINVALIDSTATE表示存在非法的参数，后两者暂略（TODO）。
        [DllImport("zookeeper.dll", EntryPoint = "zoo_awexists", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_awexists(IntPtr zh, string path,
                                watcher_fn watcher, IntPtr watcherCtx,
                                stat_completion_t completion, IntPtr data);

        //---------------------------------------------------------------------
        [DllImport("zookeeper.dll", EntryPoint = "zoo_aget", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_aget(IntPtr zh, string path, int watch,
                            data_completion_t completion, IntPtr data);

        //---------------------------------------------------------------------
        [DllImport("zookeeper.dll", EntryPoint = "zoo_awget", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_awget(IntPtr zh, string path,
                             watcher_fn watcher, IntPtr watcherCtx,
                             data_completion_t completion, IntPtr data);

        //---------------------------------------------------------------------
        [DllImport("zookeeper.dll", EntryPoint = "zoo_aset", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_aset(IntPtr zh, string path,
                            string buffer, int buflen, int version,
                            stat_completion_t completion, IntPtr data);

        //---------------------------------------------------------------------
        [DllImport("zookeeper.dll", EntryPoint = "zoo_aget_children", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_aget_children(IntPtr zh, string path,
                                     int watch,
                                     strings_completion_t completion,
                                     IntPtr data);

        //---------------------------------------------------------------------
        [DllImport("zookeeper.dll", EntryPoint = "zoo_awget_children", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_awget_children(IntPtr zh, string path,
                                      watcher_fn watcher, IntPtr watcherCtx,
                                      strings_completion_t completion,
                                      IntPtr data);

        //---------------------------------------------------------------------
        [DllImport("zookeeper.dll", EntryPoint = "zoo_aget_children2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_aget_children2(IntPtr zh, string path,
                                      int watch,
                                      strings_stat_completion_t completion,
                                      IntPtr data);

        //---------------------------------------------------------------------
        [DllImport("zookeeper.dll", EntryPoint = "zoo_awget_children2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_awget_children2(IntPtr zh, string path,
                                       watcher_fn watcher, IntPtr watcherCtx,
                                       strings_stat_completion_t completion,
                                       IntPtr data);

        //---------------------------------------------------------------------
        [DllImport("zookeeper.dll", EntryPoint = "zoo_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_async(IntPtr zh, string path,
                             string_completion_t completion, IntPtr data);

        //---------------------------------------------------------------------
        //获取当前 Zookeeper 连接状态
        [DllImport("zookeeper.dll", EntryPoint = "zoo_state", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_state(IntPtr zh);

        //---------------------------------------------------------------------
        //返回某一错误码的字符串表示。
        [DllImport("zookeeper.dll", EntryPoint = "zerror", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr zerror(int c);

        //---------------------------------------------------------------------
        //检查当前 Zookeeper 连接是否为不可恢复的，如果不可恢复，则客户端需要关闭连接，然后重连
        //[DllImport("zookeeper.dll", EntryPoint = "is_unrecoverable", CallingConvention = CallingConvention.Cdecl)]
        //public static extern  int is_unrecoverable(IntPtr zh);

        //---------------------------------------------------------------------
        [DllImport("zookeeper.dll", EntryPoint = "zookeeper_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern void zookeeper_close(IntPtr zh);

        //---------------------------------------------------------------------
        [DllImport("zookeeper.dll", EntryPoint = "zoo_amulti", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_amulti(IntPtr zh, int count, IntPtr ops,
                      IntPtr results, void_completion_t completion,
                      IntPtr data);

        //---------------------------------------------------------------------
        // 同步接口，用于初始化.
        [DllImport("zookeeper.dll", EntryPoint = "zoo_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_create(IntPtr zh, string path, string value,
            int valuelen, IntPtr acl, int flags, IntPtr path_buffer, int path_buffer_len);

        //---------------------------------------------------------------------
        [DllImport("zookeeper.dll", EntryPoint = "zoo_delete", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_delete(IntPtr zh, string path, int version);

        //---------------------------------------------------------------------
        [DllImport("zookeeper.dll", EntryPoint = "zoo_get", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_get(IntPtr zh, string path, int watch, IntPtr buffer,
                   IntPtr buffer_len, IntPtr stat);

        //---------------------------------------------------------------------
        [DllImport("zookeeper.dll", EntryPoint = "zoo_set", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_set(IntPtr zh, string path, string buffer, int buflen, int version);

        //---------------------------------------------------------------------
        [DllImport("zookeeper.dll", EntryPoint = "zoo_get_children", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_get_children(IntPtr zh, string path, int watch, IntPtr strings);

        //---------------------------------------------------------------------
        [DllImport("zookeeper.dll", EntryPoint = "zoo_exists", CallingConvention = CallingConvention.Cdecl)]
        public static extern int zoo_exists(IntPtr zh, string path, int watch, IntPtr stat);
    }
}
