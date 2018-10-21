package main

import (
	"encoding/json"
	"fmt"
	"io/ioutil"
	"math"
	"net/http"
	"os"
	"strconv"
	"strings"
)

const (
	depDate          = "2018-10-25"
	airportsEndpoint = "https://api.ryanair.com/aggregate/3/common?embedded=airports&market=en-gb"
	faresEndpoint    = "https://api.ryanair.com/farefinder/3/oneWayFares?market=en-gb&outboundDepartureDateFrom=" + depDate + "&outboundDepartureDateTo=" + depDate + "&departureAirportIataCode="

	nodesFilename          = "nodes.txt"
	availabilitiesFilename = "availabilities.txt"
	distancesFilename      = "distances.txt"
	pricesFilename         = "prices.txt"
)

type airportsResp struct {
	Airports []airport `json: "airports"`
}

type airport struct {
	IataCode    string      `json: "iataCode"`
	Name        string      `json: "name"`
	Coordinates coordinates `json: "coordinates"`
}

type coordinates struct {
	Latitude  float64 `json: "latitude"`
	Longitude float64 `json: "longitude"`
}

type faresResp struct {
	Fares []fare `json: "fares"`
}

type fare struct {
	Outbound outbound `json: "outbound"`
}

type outbound struct {
	ArrivalAirport arrivalAirport `json: "arrivalAirport"`
	Price          price          `json: "price"`
}

type arrivalAirport struct {
	IataCode string `json: "iataCode"`
}

type price struct {
	Value float64 `json: "value"`
}

func main() {
	airports, err := getAirports()
	if err != nil {
		fmt.Println("ERROR", err)
		return
	}

	avail, dist, prices, err := getGraphs(airports)
	if err != nil {
		fmt.Println("ERROR", err)
		return
	}

	err = generateFile(avail, availabilitiesFilename)
	if err != nil {
		fmt.Println("ERROR", err)
		return
	}

	err = generateFile(dist, distancesFilename)
	if err != nil {
		fmt.Println("ERROR", err)
		return
	}

	err = generateFile(prices, pricesFilename)
	if err != nil {
		fmt.Println("ERROR", err)
		return
	}

	fmt.Println("Graphs generated succesfully!")
}

func generateFile(graph [][]string, filename string) error {
	nodesG := make([]string, 0)

	for i := range graph {
		nodeS := strings.Join(graph[i], ",")
		nodesG = append(nodesG, nodeS)
	}

	file, err := os.Create(filename)
	if err != nil {
		return err
	}
	defer file.Close()

	fmt.Fprintf(file, strings.Join(nodesG, ";"))
	return nil
}

func getGraphs(airports []airport) ([][]string, [][]string, [][]string, error) {
	avail := make([][]string, len(airports))
	dist := make([][]string, len(airports))
	prices := make([][]string, len(airports))

	for i := range airports {
		avail[i] = make([]string, len(airports))
		dist[i] = make([]string, len(airports))
		prices[i] = make([]string, len(airports))

		for j := range airports {
			avail[i][j] = "-1"
			dist[i][j] = "-1"
			prices[i][j] = "-1"
		}
	}

	for i := range airports {
		fmt.Println(fmt.Sprintf("Checking availability for: %s %d/%d", airports[i].IataCode, i+1, len(airports)))
		reqBody, err := makeRequest(fmt.Sprintf("%s%s", faresEndpoint, airports[i].IataCode))
		if err != nil {
			return nil, nil, nil, err
		}

		resp := faresResp{}
		err = json.Unmarshal(reqBody, &resp)
		if err != nil {
			return nil, nil, nil, err
		}

		for j := range resp.Fares {
			availIndex := findByCode(airports, resp.Fares[j].Outbound.ArrivalAirport.IataCode)
			avail[i][availIndex] = "1"
			prices[i][availIndex] = strconv.FormatFloat(resp.Fares[j].Outbound.Price.Value, 'f', 2, 64)
			dist[i][availIndex] = getDistance(airports[i].Coordinates, airports[availIndex].Coordinates)
		}
	}

	return avail, dist, prices, nil
}

func hsin(theta float64) float64 {
	return math.Pow(math.Sin(theta/2), 2)
}

func getDistance(coords1, coords2 coordinates) string {
	var la1, lo1, la2, lo2, r float64
	la1 = coords1.Latitude * math.Pi / 180
	lo1 = coords1.Longitude * math.Pi / 180
	la2 = coords2.Latitude * math.Pi / 180
	lo2 = coords2.Longitude * math.Pi / 180

	r = 6378.1 // Earth radius km

	h := hsin(la2-la1) + math.Cos(la1)*math.Cos(la2)*hsin(lo2-lo1)

	res := 2 * r * math.Asin(math.Sqrt(h))
	return strconv.FormatFloat(res, 'f', 2, 64)
}

func getAirports() ([]airport, error) {
	reqBody, err := makeRequest(airportsEndpoint)
	if err != nil {
		return nil, err
	}

	resp := airportsResp{}
	err = json.Unmarshal(reqBody, &resp)
	if err != nil {
		return nil, err
	}

	err = generateNodesFile(resp.Airports)
	if err != nil {
		return nil, err
	}

	return resp.Airports, nil
}

func makeRequest(endpoint string) ([]byte, error) {
	req, err := http.Get(endpoint)
	if err != nil {
		return nil, err
	}
	defer req.Body.Close()

	body, err := ioutil.ReadAll(req.Body)
	if err != nil {
		return nil, err
	}

	return body, nil
}

func findByCode(airports []airport, code string) int {
	for i := range airports {
		if airports[i].IataCode == code {
			return i
		}
	}

	return -1
}

func generateNodesFile(airports []airport) error {
	names := make([]string, 0)
	for i := range airports {
		names = append(names, airports[i].Name)
	}

	file, err := os.Create(nodesFilename)
	if err != nil {
		return err
	}
	defer file.Close()

	fmt.Fprintf(file, strings.Join(names, ";"))
	return nil
}
