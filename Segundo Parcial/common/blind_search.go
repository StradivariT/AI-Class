package common

import (
	"fmt"
	"strconv"
	"strings"
)

const (
	searchErrorMsg = "there is not a possible route between those nodes"
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
		root, end, err = getMazeStart(weights)
	} else {
		root, end, err = getGraphStartAndEnd(nodes)
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

	return fmt.Errorf(searchErrorMsg)
}

func getMazeStart(weights [][]float64) (*MazeNode, string, error) {
	start := GetOption("Where would you like to start?")
	coords := strings.Split(start, ",")

	rowStart, _ := strconv.Atoi(coords[0])
	columnStart, _ := strconv.Atoi(coords[1])

	rowStart--
	columnStart--

	end := GetOption("What position would you like to find?")
	coords = strings.Split(end, ",")

	rowEnd, _ := strconv.Atoi(coords[0])
	columnEnd, _ := strconv.Atoi(coords[1])

	rowEnd--
	columnEnd--

	end = fmt.Sprintf("%d,%d", rowEnd, columnEnd)

	if rowStart >= len(weights) || columnStart >= len(weights[rowStart]) {
		return nil, "", fmt.Errorf(searchErrorMsg)
	}

	valueStart := strconv.FormatFloat(weights[rowStart][columnStart], 'f', 2, 64)

	if valueStart == "-1.00" {
		return nil, "", fmt.Errorf(searchErrorMsg)
	}

	return &MazeNode{
		Row:    rowStart,
		Column: columnStart,
		Value:  valueStart,
		Label:  0,
		Level:  0,
	}, end, nil
}

func getGraphStartAndEnd(nodes []string) (*GraphNode, string, error) {
	start := GetOption("From which node would you like to start?")
	end := GetOption("Which node would you like to find?")

	startIndex := FindInSlice(nodes, start)
	if startIndex == -1 {
		return nil, "", fmt.Errorf(searchErrorMsg)
	}

	return &GraphNode{
		Name:  nodes[startIndex],
		Index: 0,
		Label: 0,
		Level: 0,
	}, end, nil
}
