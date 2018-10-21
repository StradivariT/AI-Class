package iterative

import (
	"strconv"

	"../common"
	"../deep"
	"../limited"
)

// Search by iterative deepness
func Search(nodes []string, weights [][]float64) error {
	limit, err := limited.GetLimit()
	if err != nil {
		return err
	}

	rate, err := getRate()
	if err != nil {
		return err
	}

	err = common.BlindSearch(nodes, weights, &deep.Stack{}, limit, rate)
	if err != nil {
		return err
	}

	return nil
}

func getRate() (int64, error) {
	option := common.GetOption("What would the iterative rate be?")
	rate, err := strconv.ParseInt(option, 10, 64)
	if err != nil {
		return -1, err
	}

	return rate, nil
}
