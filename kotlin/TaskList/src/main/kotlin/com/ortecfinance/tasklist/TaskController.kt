package com.ortecfinance.tasklist

import org.springframework.web.bind.annotation.GetMapping
import org.springframework.web.bind.annotation.RequestMapping
import org.springframework.web.bind.annotation.RestController

@RestController
@RequestMapping("/tasks")
class TaskController {
    @GetMapping
    fun getTasks(): List<String> = mutableListOf("Task 1", "Task 2", "Task 3")
}
