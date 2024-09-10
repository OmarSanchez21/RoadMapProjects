using System;
using TaskManager_CLI;

namespace TaskTrackerCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = "tasks.json";
            var taskManager = new TaskManager(filePath);

            if (args.Length == 0)
            {
                Console.WriteLine("Uso: task-cli <comando> [argumentos]");
                return;
            }

            var command = args[0].ToLower();

            switch (command)
            {
                case "add":
                    AddTask(args, taskManager);
                    break;
                case "update":
                    UpdateTask(args, taskManager);
                    break;
                case "delete":
                    DeleteTask(args, taskManager);
                    break;
                case "mark-in-progress":
                    MarkInProgress(args, taskManager);
                    break;
                case "mark-done":
                    MarkDone(args, taskManager);
                    break;
                case "list":
                    ListTasks(args, taskManager);
                    break;
                default:
                    Console.WriteLine("Comando no reconocido. Los comandos disponibles son: add, update, delete, mark-in-progress, mark-done, list.");
                    break;
            }
        }

        static void AddTask(string[] args, TaskManager taskManager)
        {
            if (args.Length > 1)
            {
                var description = string.Join(" ", args[1..]);
                taskManager.AddTask(description);
                Console.WriteLine("Tarea agregada con éxito.");
            }
            else
            {
                Console.WriteLine("Error: Debes proporcionar una descripción para la tarea.");
            }
        }

        static void UpdateTask(string[] args, TaskManager taskManager)
        {
            if (args.Length > 2 && int.TryParse(args[1], out int id))
            {
                var newDescription = string.Join(" ", args[2..]);
                taskManager.UpdateTask(id, newDescription);
                Console.WriteLine("Tarea actualizada con éxito.");
            }
            else
            {
                Console.WriteLine("Error: Debes proporcionar un ID válido y una nueva descripción.");
            }
        }

        static void DeleteTask(string[] args, TaskManager taskManager)
        {
            if (args.Length > 1 && int.TryParse(args[1], out int id))
            {
                taskManager.DeleteTask(id);
                Console.WriteLine("Tarea eliminada con éxito.");
            }
            else
            {
                Console.WriteLine("Error: Debes proporcionar un ID válido.");
            }
        }

        static void MarkInProgress(string[] args, TaskManager taskManager)
        {
            if (args.Length > 1 && int.TryParse(args[1], out int id))
            {
                taskManager.MarkInProgress(id);
                Console.WriteLine("Tarea marcada como en progreso.");
            }
            else
            {
                Console.WriteLine("Error: Debes proporcionar un ID válido.");
            }
        }

        static void MarkDone(string[] args, TaskManager taskManager)
        {
            if (args.Length > 1 && int.TryParse(args[1], out int id))
            {
                taskManager.MarkDone(id);
                Console.WriteLine("Tarea marcada como hecha.");
            }
            else
            {
                Console.WriteLine("Error: Debes proporcionar un ID válido.");
            }
        }

        static void ListTasks(string[] args, TaskManager taskManager)
        {
            if (args.Length > 1)
            {
                var status = args[1].ToLower();
                taskManager.ListTasks(status);
            }
            else
            {
                taskManager.ListTasks();
            }
        }
    }
}
