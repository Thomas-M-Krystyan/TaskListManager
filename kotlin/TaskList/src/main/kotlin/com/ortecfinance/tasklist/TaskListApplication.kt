package com.ortecfinance.tasklist

import org.springframework.boot.autoconfigure.SpringBootApplication
import org.springframework.boot.runApplication

@SpringBootApplication
class TaskListApplication

fun main(args: Array<String>) {
    if (args.isEmpty()) {
        println("Starting console Application")
        TaskList.startConsole()
    } else {
        runApplication<TaskListApplication>(*args)
        println("localhost:8080/tasks")
    }
}
