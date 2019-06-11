using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using System.IO;
//using BookSleeve;

// for StackExchange.Redis.Extensions
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace RedisNetTest
{
    class User
    {
        public string Firstname;
        public string Lastname;
        public string Twitter;
        public string Blog;
    }

    class Program
    {
        static void Main(string[] args)
        {
            RedisTest2();
        }

        // for Lazy singleton (thread-safe)
        private static Lazy<StackExchangeRedisCacheClient> lazyCacheClient = null;

        private static void RedisTest2() {        
            // 0. serializer / ICacheClient 생성            
            var serializer = new NewtonsoftSerializer();
 
            // App.config에서 설정을 하였기에, 3가지 원형중 가장 간단한 원형으로 생성
            lazyCacheClient = new Lazy<StackExchangeRedisCacheClient>(() =>
            {
                return new StackExchangeRedisCacheClient(serializer);
            });
            var cacheClient = lazyCacheClient.Value;
 
            // 1. User 객체 쓰기
            var user1 = new User()
            {
                Firstname = "Ugo",
                Lastname = "Lattanzi",
                Twitter = "@imperugo",
                Blog = "http://tostring.it"
            };
 
            // result : bool
            // Add(key, value)
            // Add(key, value, expiresIn(얼마뒤에))
            // Add(key, value, expiresAt(언제)) <<- 여기선 이걸 씀
            bool added = cacheClient.Add("user1", user1, DateTimeOffset.Now.AddMinutes(10));
            if (added)
            {
                Console.WriteLine("User1 has been updated to Redis");
            }
            else
            {
                Console.WriteLine("User1 FAILED to update to Redis");
            }
 
            // 2. User 객체 읽기
            // value = Get<ValueType>(key)
            User cachedUser = cacheClient.Get<User>("user1");
            if (cachedUser != null)
            {
                Console.WriteLine("User1 loaded from Redis");
            }
            Console.WriteLine();
 
            // 3. 한번에 여러 오브젝트 쓰기
            //    Tuple<key, value>의 List 형식
            IList<Tuple<string, User>> userList = new List<Tuple<string, User>>();
 
            var user2 = new User()
            {
                Firstname = "Simone",
                Lastname = "Chiaretta",
                Twitter = "@simonech",
                Blog = "http://codeclimber.net.nz"
            };
 
            var user3 = new User()
            {
                Firstname = "Matteo",
                Lastname = "Pagani",
                Twitter = "@qmatteoq",
                Blog = "http://qmatteoq.com"
            };
 
            userList.Add(Tuple.Create("user1", user1));
            userList.Add(Tuple.Create("user2", user2));
            userList.Add(Tuple.Create("user3", user3));
 
            bool listAdded = cacheClient.AddAll(userList);
            if (listAdded)
            {
                Console.WriteLine("UserList has been updated to Redis");
            }
            else
            {
                Console.WriteLine("UserList FAILED to update to Redis");
            }
 
            // 3. 한번에 여러 오브젝트 읽기
            string[] keys = { "user1", "user2", "user3" };            
            var cachedUserList = cacheClient.GetAll<User>(keys);
            if (cachedUserList.Count != 0)
            {
                Console.WriteLine("UserList loaded from Redis");
            }
            Console.WriteLine();
 
            // 4. 패턴을 이용한 복수의 key 찾기
            var foundKeys = cacheClient.SearchKeys("user*");
            Console.Write("Found keys : ");
            foreach (var key in foundKeys)
            {
                //Console.Write(cacheClient.Get<string>("name").ToString());
                Console.Write("{0} ", key);
            }
            Console.WriteLine();
 
            // 5. Redis 메쏘드 직접 사용
            // IDatabase를 가져와서 그냥 사용하면 된다
            IDatabase dbMaster = cacheClient.Database;
            dbMaster.StringSet("simpleKey", "simpleValue:abcdefg");
            string getValue = dbMaster.StringGet("simpleKey");
            Console.WriteLine(getValue);
            Console.WriteLine(dbMaster.StringGet("name"));
            Console.WriteLine();

            Console.WriteLine("ServerInfo more : y/n"); 
            string line = Console.ReadLine();
            if (line == "y")
            {

                // 6. 서버 정보 가져오기
                // redis의 "info" 명령어와 동일한 정보를 얻어온다
                // info는 Dictionary<string, string> 형식
                var infoDict = cacheClient.GetInfo();
                Console.WriteLine("--------------- ServerInfo --------------");
                foreach (var info in infoDict)
                {
                    Console.WriteLine("{0} : {1} ", info.Key, info.Value);
                }
            }


            Console.Read();
        }

        private static void RedisTest() {
            Console.WriteLine("Redis test");

            //var con = new RedisConnection("127.0.0.1");
            //await con.Open();
            //var x1 con.Strings.Increment(db:0, key:"X");


        }
    }

}
