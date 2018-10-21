package common

import (
	"fmt"
	"strconv"
	"strings"
)

const (
	mazeEnd = "2.00"
)

// DataStructure depends on the algorithm used
type DataStructure interface {
	Push(Finder)
	Pop() Finder
	Empty() bool
}

// Finder refers to the type of node being used, from a graph or from the maze
type Finder interface {
	AppendChildren([]string, [][]float64, DataStructure, []Finder)
	IsEnd(string) bool
	ID() string
	GetLabel() float64
	GetLevel() int64
	PrintResults()
}

// BlindSearch encapsulates the logic for both deep and amplitude search
func BlindSearch(nodes []string, weights [][]float64, dStruct DataStructure, limit, rate int64) error {
	var (
		root Finder
		err  error
		end  string
	)

	if nodes == nil {
		root, err = getMazeStart(weights)
		end = mazeEnd
	} else {
		root, end = getGraphStartAndEnd(nodes)
	}

	if err != nil {
		return err
	}

	var currLimit int64
	if rate == -1 {
		currLimit = limit
	}

	closed := []Finder{}
	dStruct.Push(root)

	for !dStruct.Empty() {
		node := dStruct.Pop()
		closed = append(closed, node)

		if node.IsEnd(end) {
			node.PrintResults()
			return nil
		}

		if limit == -1 || node.GetLevel() < currLimit {
			node.AppendChildren(nodes, weights, dStruct, closed)
		}

		if rate != -1 && dStruct.Empty() && currLimit < limit {
			closed = []Finder{}
			dStruct.Push(root)

			currLimit += rate
			if currLimit > limit {
				currLimit = limit
			}
		}
	}

	return fmt.Errorf("the destination was not found")
}

func getMazeStart(weights [][]float64) (*MazeNode, error) {
	start := GetOption(fmt.Sprintf("The map is %d rows long and %d columns long. Where would you like to start?", len(weights), len(weights[0])))
	coords := strings.Split(start, ",")

	row, err := strconv.Atoi(coords[0])
	if err != nil {
		return nil, err
	}

	column, err := strconv.Atoi(coords[1])
	if err != nil {
		return nil, err
	}

	row--
	column--

	value := strconv.FormatFloat(weights[row][column], 'f', 2, 64)

	if value == "-1.00" {
		return nil, fmt.Errorf("that position is an invalid block")
	}

	return &MazeNode{
		Row:    row,
		Column: column,
		Value:  value,
		Label:  0,
		Level:  0,
	}, nil
}

func getGraphStartAndEnd(nodes []string) (*GraphNode, string) {
	end := GetOption("Which airport would you like to find?")

	return &GraphNode{
		Name:  nodes[0],
		Index: 0,
		Label: 0,
		Level: 0,
	}, end
}
