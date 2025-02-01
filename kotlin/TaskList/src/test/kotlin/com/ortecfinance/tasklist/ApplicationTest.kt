package com.ortecfinance.tasklist

import org.hamcrest.MatcherAssert
import org.hamcrest.Matchers
import org.junit.jupiter.api.AfterEach
import org.junit.jupiter.api.BeforeEach
import java.io.*
import kotlin.test.Test

class ApplicationTest {
    private val inStream = PipedOutputStream()
    private val inWriter = PrintWriter(inStream, true)

    private val outStream = PipedInputStream()
    private val outReader = BufferedReader(InputStreamReader(outStream))

    private val applicationThread: Thread

    init {
        val `in` = BufferedReader(InputStreamReader(PipedInputStream(inStream)))
        val out = PrintWriter(PipedOutputStream(outStream), true)
        val taskList = TaskList(`in`, out)
        applicationThread = Thread(taskList)
    }

    @BeforeEach
    @Throws(IOException::class)
    fun start_the_application() {
        applicationThread.start()
        readLines("Welcome to TaskList! Type 'help' for available commands.")
    }

    @AfterEach
    @Throws(IOException::class, InterruptedException::class)
    fun kill_the_application() {
        if (!stillRunning()) {
            return
        }

        Thread.sleep(1000)
        if (!stillRunning()) {
            return
        }

        applicationThread.interrupt()
        throw IllegalStateException("The application is still running.")
    }

    @Test
    @Throws(IOException::class)
    fun it_works() {
        execute("show")

        execute("add project secrets")
        execute("add task secrets Eat more donuts.")
        execute("add task secrets Destroy all humans.")

        execute("show")
        readLines(
            "secrets",
            "    [ ] 1: Eat more donuts.",
            "    [ ] 2: Destroy all humans.",
            ""
        )

        execute("add project training")
        execute("add task training Four Elements of Simple Design")
        execute("add task training SOLID")
        execute("add task training Coupling and Cohesion")
        execute("add task training Primitive Obsession")
        execute("add task training Outside-In TDD")
        execute("add task training Interaction-Driven Design")

        execute("check 1")
        execute("check 3")
        execute("check 5")
        execute("check 6")

        execute("show")
        readLines(
            "secrets",
            "    [x] 1: Eat more donuts.",
            "    [ ] 2: Destroy all humans.",
            "",
            "training",
            "    [x] 3: Four Elements of Simple Design",
            "    [ ] 4: SOLID",
            "    [x] 5: Coupling and Cohesion",
            "    [x] 6: Primitive Obsession",
            "    [ ] 7: Outside-In TDD",
            "    [ ] 8: Interaction-Driven Design",
            ""
        )

        execute("quit")
    }

    @Throws(IOException::class)
    private fun execute(command: String) {
        read(PROMPT)
        write(command)
    }

    @Throws(IOException::class)
    private fun read(expectedOutput: String) {
        val length = expectedOutput.length
        val buffer = CharArray(length)
        outReader.read(buffer, 0, length)
        MatcherAssert.assertThat(String(buffer), Matchers.`is`(expectedOutput))
    }

    @Throws(IOException::class)
    private fun readLines(vararg expectedOutput: String) {
        for (line in expectedOutput) {
            read(line + System.lineSeparator())
        }
    }

    private fun write(input: String) {
        inWriter.println(input)
    }

    private fun stillRunning(): Boolean {
        return applicationThread.isAlive
    }

    companion object {
        const val PROMPT: String = "> "
    }
}
