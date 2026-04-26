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


Projects are stored as JSON entries within `projects_list.json`, located in the application's persistent data directory:
```json
{
  "name": "FirstProject",
  "path": "path\\to\\FirstProject",
  "createdDate": "4/26/2026 5:08:37 PM",
  "lastModified": "4/26/2026 5:08:37 PM"
}
```

---

## Project Structure

The project directory includes:

- `graph.json`: defines the node graph used for authoring and editing in Oasis  
- `Resources/`: contains scene background assets stored as `*.bg` and `seq.json`
- `seq.json`: defines the execution sequence from the Start node to the End node  

## Portability & Preview

This separation allows experiences to be portable across platforms. Runtime preview only depends on the `Resources` directory.

Preview currently runs inside the application. The same preview logic can be reused to build standalone targets (e.g., Android or HTML5).

---

## Node Types

- Start Node: Entry point of the graph
- End Node: Terminates execution
- Talk Node: Renders dialogue or instructional text
- Scene Node: Loads a background to define context
- Logic Node (True / False): Evaluates a condition and routes execution
  
---

## Controls

- Right-click (single): remove connections from a node  
- Right-click (double): delete node (except the start & end node)
- Left-click (double): to edit node data (except the start & end node)
- Left-click and hold: to drag and move a node

---

## How to Run

### Option 1 — Prebuilt
Use the compressed build `Oasis_v0.0.1`


### Option 2 — Unity Editor
Open the project in Unity (6000.3.10f1)  and press play  

---

## First Project
`FirstProject` contains basic example flows, use the **Add** button to import and test it.
