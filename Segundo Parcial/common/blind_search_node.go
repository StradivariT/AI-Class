package common

import (
	"fmt"
	"strconv"
)

// GraphNode -
type GraphNode struct {
	Name   string
	Index  int
	Label  float64
	Level  int64
	Parent *GraphNode
}

// MazeNode -
type MazeNode struct {
	Row    int
	Column int
	Value  string
	Label  float64
	Level  int64
	Parent *MazeNode
}

// AppendChildren to the corresponding DataStructure if conditions meet
func (g *GraphNode) AppendChildren(nodes []string, weights [][]float64, dStruct DataStructure, closed []Finder) {
	for i := range weights[g.Index] {
		if weights[g.Index][i] == -1 || isClosed(nodes[i], closed) {
			continue
		}

		dStruct.Push(&GraphNode{
			Name:   nodes[i],
			Index:  i,
			Label:  g.GetLabel() + weights[g.Index][i],
			Level:  g.GetLevel() + 1,
			Parent: g,
		})
	}
}

// IsEnd if the ID is the same as the node's name
func (g *GraphNode) IsEnd(name string) bool {
	if g.ID() == name {
		return true
	}

	return false
}

// ID returns the node's name
func (g *GraphNode) ID() string {
	return g.Name
}

// GetLabel -
func (g *GraphNode) GetLabel() float64 {
	return g.Label
}

// GetLevel -
func (g *GraphNode) GetLevel() int64 {
	return g.Level
}

// PrintResults from end to start
func (g *GraphNode) PrintResults() {
	node := g

	for node != nil {
		msg := fmt.Sprintf("Node: %s  Label: %.2f", node.Name, node.Label)
		fmt.Println(msg)

		node = node.Parent
	}
}

// AppendChildren to the corresponding DataStructure if conditions meet
func (m *MazeNode) AppendChildren(nodes []string, weights [][]float64, dStruct DataStructure, closed []Finder) {
	maxRow := len(weights) - 1
	maxColumn := len(weights[0]) - 1

	// Bottom
	if m.Row != maxRow && weights[m.Row+1][m.Column] != -1 && !isClosed(fmt.Sprintf("%d,%d", m.Row+1, m.Column), closed) {
		value := strconv.FormatFloat(weights[m.Row+1][m.Column], 'f', 2, 64)

		dStruct.Push(&MazeNode{
			Row:    m.Row + 1,
			Column: m.Column,
			Value:  value,
			Label:  m.GetLabel() + weights[m.Row+1][m.Column],
			Level:  m.GetLevel() + 1,
			Parent: m,
		})
	}

	// Right
	if m.Column != maxColumn && weights[m.Row][m.Column+1] != -1 && !isClosed(fmt.Sprintf("%d,%d", m.Row, m.Column+1), closed) {
		value := strconv.FormatFloat(weights[m.Row][m.Column+1], 'f', 2, 64)

		dStruct.Push(&MazeNode{
			Row:    m.Row,
			Column: m.Column + 1,
			Value:  value,
			Label:  m.GetLabel() + weights[m.Row][m.Column+1],
			Level:  m.GetLevel() + 1,
			Parent: m,
		})
	}

	// Up
	if m.Row != 0 && weights[m.Row-1][m.Column] != -1 && !isClosed(fmt.Sprintf("%d,%d", m.Row-1, m.Column), closed) {
		value := strconv.FormatFloat(weights[m.Row-1][m.Column], 'f', 2, 64)

		dStruct.Push(&MazeNode{
			Row:    m.Row - 1,
			Column: m.Column,
			Value:  value,
			Label:  m.GetLabel() + weights[m.Row-1][m.Column],
			Level:  m.GetLevel() + 1,
			Parent: m,
		})
	}

	// Left
	if m.Column != 0 && weights[m.Row][m.Column-1] != -1 && !isClosed(fmt.Sprintf("%d,%d", m.Row, m.Column-1), closed) {
		value := strconv.FormatFloat(weights[m.Row][m.Column-1], 'f', 2, 64)

		dStruct.Push(&MazeNode{
			Row:    m.Row,
			Column: m.Column - 1,
			Value:  value,
			Label:  m.GetLabel() + weights[m.Row][m.Column-1],
			Level:  m.GetLevel() + 1,
			Parent: m,
		})
	}
}

// IsEnd if the ID is the same as the node's value
func (m *MazeNode) IsEnd(id string) bool {
	if m.Value == id {
		return true
	}

	return false
}

// ID returns the coordinates as string
func (m *MazeNode) ID() string {
	return fmt.Sprintf("%d,%d", m.Row, m.Column)
}

// GetLabel -
func (m *MazeNode) GetLabel() float64 {
	return m.Label
}

// GetLevel -
func (m *MazeNode) GetLevel() int64 {
	return m.Level
}

// PrintResults coordinates from end to start
func (m *MazeNode) PrintResults() {
	node := m

	for node != nil {
		msg := fmt.Sprintf("Coord: %d,%d  Label: %.2f", node.Row+1, node.Column+1, node.Label)
		fmt.Println(msg)

		node = node.Parent
	}
}

func isClosed(id string, closed []Finder) bool {
	for i := range closed {
		if closed[i].ID() == id {
			return true
		}
	}

	return false
}
