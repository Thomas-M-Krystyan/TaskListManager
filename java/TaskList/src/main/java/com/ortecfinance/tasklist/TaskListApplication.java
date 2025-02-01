package com.ortecfinance.tasklist;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

@SpringBootApplication
public class TaskListApplication {

    public static void main(String[] args) {
        if (args.length == 0) {
            System.out.println("Starting console Application");
            TaskList.startConsole();
        }
        else {
            SpringApplication.run(TaskListApplication.class, args);
            System.out.println("localhost:8080/tasks");
        }
    }

}
