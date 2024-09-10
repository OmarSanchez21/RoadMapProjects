using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
namespace TaskManager_CLI
{
    class Task
    {
        public int Id {get;set;}
        public string Description {get;set;}
        public string Status {get;set;}
        public DateTime CreatedAt {get;set;}
        public DateTime UpdatedAt {get;set;}
    }
    class TaskManager
    {
        private string filePath;
        private List<Task> tasks;
        public TaskManager(string filePath)
        {
            this.filePath = filePath;
            LoadTask();
        }

        private void LoadTask()
        {
            if(File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                tasks = JsonConvert.DeserializeObject<List<Task>>(json) ?? new List<Task>();
            }
            else
            {
                tasks = new List<Task>();
            }
        }
        private void SaveTask()
        {
            var json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
            File.WriteAllText(filePath, json);     
        }
        public void AddTask(string description)
        {
            var newTask = new Task
            {
                Id = tasks.Count > 0 ? tasks[tasks.Count - 1].Id + 1 : 1,
                Description = description,
                Status = "todo",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            tasks.Add(newTask);
            SaveTask();
        }
        public void UpdateTask(int id, string newDescription)
        {
            var task = tasks.Find(t => t.Id == id);
            if(task != null)
            {
                task.Description = newDescription;
                task.UpdatedAt = DateTime.Now;
                SaveTask();
            }
        }
        public void DeleteTask(int id)
        {
            var task = tasks.Find(t => t.Id == id);
            if(task != null)
            {
                tasks.Remove(task);
                SaveTask();
            }
        }
        public void MarkInProgress(int id)
        {
            var task = tasks.Find(t => t.Id == id);
            if (task != null)
            {
                task.Status = "in-progress";
                task.UpdatedAt = DateTime.Now;
                SaveTask();
            }
        }

        public void MarkDone(int id)
        {
            var task = tasks.Find(t => t.Id == id);
            if (task != null)
            {
                task.Status = "done";
                task.UpdatedAt = DateTime.Now;
                SaveTask();
            }
        }

        public void ListTasks(string status = null)
        {
            IEnumerable<Task> taskList = tasks;

            if (status != null)
            {
                taskList = tasks.FindAll(t => t.Status == status);
            }

            foreach (var task in taskList)
            {
                Console.WriteLine($"ID: {task.Id}, Description: {task.Description}, Status: {task.Status}, Created At: {task.CreatedAt}, Updated At: {task.UpdatedAt}");
            }
        }
    }
}