using System;

using System.Threading.Tasks;
using Model;
using System.Net.Http;
using System.Reflection;
using System.Collections.Generic;

namespace WebApiAccess
{
    class ApiCalls
    {
        static HttpClient client;
        public ApiCalls(string baseAddress)
        {
            client = new HttpClient();
            baseAddress += "/api/values/";
            client.BaseAddress = new Uri(baseAddress);
        }
        public async void ReadInput()
        {
            while (true)
            {
                Console.WriteLine("1. getstudents \t 2.getgroups \t 3.getstudentByid \t  4.getgroupbyid \n 5.addstudent\t6.addgroup\t7.getStudentByName\n8.getStudentByRank\n9.exit");
                int choice = Convert.ToInt32(Console.ReadLine());
                IEnumerable<object> resultToPrint;
                switch (choice)
                {
                    case 1: { resultToPrint = Get<Student>("getstudents").GetAwaiter().GetResult(); print<Student>((Student[])resultToPrint); } break;
                    case 2: { resultToPrint = Get<Group>("getgroups").GetAwaiter().GetResult(); print<Group>((Group[])resultToPrint); } break;
                    case 3:
                        {

                            Console.WriteLine("enter student id"); int studentId = Convert.ToInt32(Console.ReadLine());
                            resultToPrint = Get<Student>("getstudentbyid/?id=" + studentId).GetAwaiter().GetResult();
                            print<Student>((Student[])resultToPrint);
                        }
                        break;
                    case 4:
                        {
                            Console.WriteLine("enter group id"); int groupId = Convert.ToInt32(Console.ReadLine());
                            resultToPrint = Get<Group>("getgroupbyid/?id=" + groupId).GetAwaiter().GetResult();
                            print<Group>((Group[])resultToPrint);
                        }
                        break;
                    case 5:
                        {
                            Console.WriteLine("enter student name"); string studentName = Console.ReadLine();
                            Console.WriteLine("enter student id"); int studentId = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("enter student rank"); int studentRank = Convert.ToInt32(Console.ReadLine());
                            var student = new Student() { StudentName = studentName, StudentId = studentId, StudentRank = studentRank };
                            var HttpResult = Post<Student>(student).GetAwaiter().GetResult();
                            if (HttpResult.IsSuccessStatusCode == true) Console.WriteLine("value inserted");
                            else Console.WriteLine("value already exists in database,try a different one!");
                        }
                        break;
                    case 6:
                        {
                            Console.WriteLine("enter Group name"); string groupName = Console.ReadLine();
                            Console.WriteLine("enter group id"); int groupId = Convert.ToInt32(Console.ReadLine());
                            var group = new Group() { GroupName = groupName, GroupId = groupId };
                            var HttpResult = Post<Group>(group).GetAwaiter().GetResult();
                            if (HttpResult.IsSuccessStatusCode == true) Console.WriteLine("value inserted");
                            else Console.WriteLine("value already exists in database,try a different one!");

                        }
                        break;
                    case 7:
                        {
                            Console.WriteLine("enter student name"); string studentName = Console.ReadLine();
                            resultToPrint = Get<Student>("getstudentbyname/?name=" + studentName).GetAwaiter().GetResult();
                            print<Student>((Student[])resultToPrint);
                        }
                        break;
                    case 8:
                        {
                            Console.WriteLine("enter student rank"); int studentRank = Convert.ToInt32(Console.ReadLine());
                            resultToPrint = Get<Student>("getstudentbyrank/?rank=" + studentRank).GetAwaiter().GetResult();
                            print<Student>((Student[])resultToPrint);
                        }
                        break;
                    case 9: Environment.Exit(0); break;
                }
            }
        }
        /// <summary>
        /// using json Asynchronously posts the values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> Post<T>(T obj)
        {
            //appends addstudent or addgroup based on type T
            var postTask = await client.PostAsJsonAsync<T>("add" + typeof(T).Name.ToLower(), obj);
            return postTask;
        }


        public static async Task<IEnumerable<T>> Get<T>(String url) where T : new()
        {

            var responseTask = client.GetAsync(url);
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                //result contains an array containing student elements or group elements based on type T 
                if (!url.Contains("?"))
                {
                    var readTask = await result.Content.ReadAsAsync<T[]>();
                    return readTask;
                }
                else
                {
                    //result contains an object of type T
                    var readTask = await result.Content.ReadAsAsync<T>();
                    //storing the resultant in an array
                    T[] arr = new T[] { readTask };
                    return arr;
                }
            }
            else
            {
                Console.WriteLine("element not found");
                Console.ReadLine();
                return null;
            }
        }
        /// <summary>
        /// prints all the elements in array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        public static void print<T>(IEnumerable<T> result)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (var element in result)
            {
                foreach (var property in properties)
                {
                    Console.Write(property.GetValue(element) + "      ");
                }
                Console.WriteLine();
            }
        }
    }
}
