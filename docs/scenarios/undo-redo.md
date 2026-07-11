# Undo / Redo (Stack<T>)

## Scenario

Track user actions so they can be undone and redone (e.g., text editor, UI operations, commands).

---

## Problem

You need:
- last-in-first-out behavior
- ability to revert recent actions
- ability to restore undone actions

---

## Solution

Use two stacks:

`csharp
var undoStack = new Stack<string>();
var redoStack = new Stack<string>();

void Do(string action)
{
    undoStack.Push(action);
    redoStack.Clear();
}

string Undo()
{
    if (undoStack.Count == 0) return null;

    var action = undoStack.Pop();
    redoStack.Push(action);
    return action;
}

string Redo()
{
    if (redoStack.Count == 0) return null;

    var action = redoStack.Pop();
    undoStack.Push(action);
    return action;
}
`

---

## Why it works

- Stack<T> provides LIFO behavior
- Undo removes most recent action
- Redo restores last undone action

---

## When to use

- UI actions
- command history
- reversible operations

---

## When NOT to use

- need random access
- need ordered iteration

---

## The trap

Using List<T> instead of Stack<T>:

- requires manual index management
- easier to introduce bugs

Stack<T> expresses intent clearly.

---

## Rule of thumb

Use Stack<T> when you need to reverse recent actions (undo/redo).
