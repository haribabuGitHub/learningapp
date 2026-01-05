// See https://aka.ms/new-console-template for more information
using RedisCacheAzureService;
using StackExchange.Redis;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;

Console.WriteLine("Hello, World!");

//string connectionString = "";




ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);

IDatabase db = connectionMultiplexer.GetDatabase();

db.StringSet("top3courses", "C#,Java,Phython");

Console.WriteLine(db.KeyExists("top3courses"));

Console.WriteLine(db.StringGet("top3courses"));

List<Course> courses = new List<Course>();

courses.Add(new Course() { CourseId = 101, CourseName = "C#", CourseDescription = "C# Basics", Price = 9.99f });

courses.Add(new Course() { CourseId = 102, CourseName = "Java", CourseDescription = "Java Basics", Price = 8.99f });

courses.Add(new Course() { CourseId = 103, CourseName = "Phython", CourseDescription = "Phython Basics", Price = 7.99f });

//foreach (var course in courses)
//    db.ListLeftPush("Courses", JsonSerializer.Serialize(course));

long listLength = db.ListLength("Courses");

Console.WriteLine(listLength);

for (int i = 0; i < listLength; i++)
{
    string courseJson = db.ListRightPop("Courses");
    Course course = JsonSerializer.Deserialize<Course>(courseJson);
    Console.WriteLine($"{course.CourseId} - {course.CourseName} - {course.CourseDescription} - {course.Price}");
}





