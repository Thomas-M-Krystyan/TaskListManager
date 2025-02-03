# Task List

You have been handed over an existing codebase of a task list application. This application allows you to create projects, add tasks to those projects, check and uncheck them, and view tasks by project.
There are also some unit tests present in the codebase, which should give you some understanding on how to application works.
Your job is to add extra features to the application, and to refactor the codebase to make it more maintainable and testable.
We expect you to take at most a few hours for this assignment.

The application is available in multiple programming languages. Pick the language you are most comfortable with.

Clone this repository and commit your changes on it. Make sure to add properly sized commits with meaningful commit messages.

## The assignment

Below are the changes you need to make. Make sure that for each change you make, you add sufficient test coverage. Feel free to refactor the code base to make it easier to test.
Make sure to not break existing functionality.

### 1. Adding deadlines to tasks

1. Add a new command `deadline <ID> <date>`. With this command you are able to add a deadline to a task. By default, a task has no deadline.
    - Example: `deadline 1 25-11-2024`  
2. Add a new command `today`. This command shows the same data as the `show` command, but it will only contain the tasks (and project it belongs to) that have a deadline for today. Make sure to not print any projects for tasks that don't have a deadline today.

### 2. Deadline view
1. Add a `view-by-deadline` command. This command will show all tasks grouped by deadline. Make sure the list is ordered chronologically, and the `no deadline` block is at the end. An example output could be:
   ```
   11-11-2021:
          1: Eat more donuts.
          4: Four Elements of Simple Design
   13-11-2021:
          3: Interaction-Driven Design
   No deadline:
          2: Refactor the codebase
   ```
2. Bonus: Also group by project after grouping by date. Example:
   ```
   11-11-2021:
   		Secrets:
          	1: Eat more donuts.
        Training:
          	4: Four Elements of Simple Design
   13-11-2021:
   		Training:
          	3: Interaction-Driven Design
   No deadline:
        Training: 
          	2: Refactor the codebase
   ```


### 3. Refactor the code base for multiple interfaces

At the moment, all interaction of the program happens via `TaskList.run` and `TaskList.execute`, since this is a command line program. However, in the future, we might want to add a GUI or a REST API that interacts with the same code. 
- Refactor the codebase to make it easier to add new interfaces in the future. Start with extracting the core logic away from the command line logic.
- Refactor the core logic further, taking the following criteria into account:
  - The core logic should be easy to test and have good test coverage
  - The classes should be small and have a single responsibility
  - If possible, split up the refactorings in small steps that can be committed separately

### 4. Create REST APIs for the application 
Currently the application is only controllable via the console. Now that the core logic is extracted, we also want interaction via REST APIs.
It's okay to keep storing the tasks in memory such as is happening now (instead of a database).

In the file `TaskListApplication` there is some logic to either start the console application or the web application. You could use the provided IDE run configurations to run either application, or modify this code manually to run either the one or the other.

Note that the console application should also keep fully working. In theory you should be able to run both the console application and the REST APIs at the same time.

You should create at least the following REST endpoints:
- `POST /projects`: Create a new project
- `POST /projects/{project_id}/tasks`: Create a new task for a project
- `PUT /projects/{project_id}/tasks/{task_id}?deadline`: Update the deadline for a task
- `GET /projects/view_by_deadline`: Get all tasks grouped by deadline (or also by project if you did the bonus)


### 5. Remarks from the author:
- All strings can be extracted to dedicated `Resources.resx` files
- The two collections (simulating tables in a database) inside of `ConcurrentTaskManager.cs` should be extracted
  to a new layer **Persistence** for example as a `TaskList.InMemory` library project
- The `TaskList.Logic` project in the Application layer, should use DTO models to communicate with Persistence
- The console application is a little buggy, and the approach to decompose commands is not always perfectly
  working. Looking into this project and how to make it 100% reliable and easier to read, would be recommended
- API project should be protected, at least, by API key (or similar solution, e.g. JWT tokens)
- 100% code coverage is possible, adding more tests would be the next step
- Using a data store (e.g., an external database), would be recommended to give users an option to edit and to
  display changes made from both applications (console and API), even at the same time.

  ![image](https://github.com/user-attachments/assets/1ef73358-b1ec-434d-aea1-55cae9dd6573)
