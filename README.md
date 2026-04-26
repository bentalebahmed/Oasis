# Oasis

Prototype project exploring node-based e-learning systems.

Oasis is a minimal 2D Virtual Training Studio for building structured training experiences through a directed node graph. Each node represents a discrete function, and execution follows a fully deterministic flow.

Built with Unity 6.3 LTS (6000.3.10f1).

---

## Features

- Project lifecycle: create, add, save, load
- Node-based authoring
- Binary branching logic (true/false)
- Real-time experience preview

---

## Node Types

### Start Node
Entry point of the graph.

### End Node
Terminates execution.

### Talk Node
Renders dialogue or instructional text.

### Scene Node
Loads a background to define context.

### Logic Node (True / False)
Evaluates a condition and routes execution.

---

## Execution Model

1. Start Node initializes execution  
2. Nodes execute sequentially  
3. Logic Nodes branch conditionally  
4. Flow terminates at an End Node  

All behavior is explicitly defined. No implicit state or hidden transitions.

---

## Controls

- Right-click (single): remove connections from a node  
- Right-click (double): delete node  

---

## How to Run

### Option 1 — Prebuilt
Use the compressed build `Oasis_v0.0.1`


### Option 2 — Unity Editor
Open the project in Unity (6000.3.10f1)  and press play  

---

## First Project
`FirstProject` contains basic example flows.  
Use the **Add** button to import and test it.
