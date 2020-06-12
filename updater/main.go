package main

import (
	"fmt"
	"log"
	"os"
	"os/exec"
	"time"
)

func main() {
	args := os.Args

	// prepare logger
	logfile, err := os.OpenFile("updater.log", os.O_CREATE|os.O_WRONLY|os.O_TRUNC, 0644)
	if err != nil {
		log.Println(err)
	}
	defer logfile.Close()

	logger := log.New(logfile, "[ electron-asar-hot-updater/updater ]", log.LstdFlags)

	if len(args) >= 3 {
		updateAsar := args[1]
		appAsar := args[2]

		logger.Printf("Will move %s to %s\n", updateAsar, appAsar)

		// check and replace app.asar
		_, updateAsarErr := os.Stat(updateAsar)
		if os.IsNotExist(updateAsarErr) {
			logger.Fatalf("Update.asar not exist.\n")
			fmt.Fprintf(os.Stderr, "Update.asar not exist.\n")
		} else {
			// wait 5 seconds for app exit
			time.Sleep(5 * time.Second)
			_, appAsarErr := os.Stat(appAsar)
			if !os.IsNotExist(appAsarErr) {
				os.Remove(appAsar)
			}
			os.Rename(updateAsar, appAsar)
			logger.Println("app.asar replacement is completed.")
		}

		// restart application
		if len(args) == 4 {
			app := args[3]
			cmd := exec.Command(app)
			cmd.Stdout = os.Stdout
			cmd.Stderr = os.Stderr
			err := cmd.Run()
			if err != nil {
				logger.Printf("app execute failed with %s\n", err)
			}
		}
	} else {
		logger.Fatalf("Wrong arguments: %s\n", args)
		fmt.Fprintf(os.Stderr, "Wrong arguments: %s\n", args)
	}
}
