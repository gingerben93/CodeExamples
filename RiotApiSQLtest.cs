//removed excess code; missing some class declarations

//This is a ASP.net web app project that uses SQL

        private void button1_Click(object sender, EventArgs e)
        {
            long MatchID = Convert.ToInt64(MatchIDTextBox.Text);
            //set request information //matchid from riot api 
            while (true)
            { 
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + MatchID.ToString() + parameters + key);
                //get response as json object an convert to string
                try
                {
                    var response = request.GetResponse();
                    var stream = response.GetResponseStream();
                    var reader = new StreamReader(stream);
                    var jsonString = reader.ReadToEnd();
                    response.Close();

                    //if possible turn json to c# class
                    Example MatchInfo = JsonConvert.DeserializeObject<Example>(jsonString);
                    if (MatchInfo.gameMode == "CLASSIC")
                    { 
                        //only care about 10 champ for now; these are current table columns
                        int champion1 = MatchInfo.participants[0].championId;
                        int champion2 = MatchInfo.participants[1].championId;
                        int champion3 = MatchInfo.participants[2].championId;
                        int champion4 = MatchInfo.participants[3].championId;
                        int champion5 = MatchInfo.participants[4].championId;
                        int champion6 = MatchInfo.participants[5].championId;
                        int champion7 = MatchInfo.participants[6].championId;
                        int champion8 = MatchInfo.participants[7].championId;
                        int champion9 = MatchInfo.participants[8].championId;
                        int champion10 = MatchInfo.participants[9].championId;
                        int win1 = 0;
                        int lose1 = 0;
                        int win2 = 0;
                        int lose2 = 0;
                        int team1ID = 0;
                        int team2ID = 0;
                        if (MatchInfo.teams[0].win == "Win")
                        {
                            win1 = 1;
                            lose1 = 0;
                            win2 = 0;
                            lose2 = 1;
                        }
                        else
                        {
                            win1 = 0;
                            lose1 = 1;
                            win2 = 1;
                            lose2 = 0;
                        }

                        //web app text boxes for visualization
                        Team1Text.Text = MatchInfo.teams[0].win.ToString();
                        Champion1Text.Text = MatchInfo.participants[0].championId.ToString();
                        Champion2Text.Text = MatchInfo.participants[1].championId.ToString();
                        Champion3Text.Text = MatchInfo.participants[2].championId.ToString();
                        Champion4Text.Text = MatchInfo.participants[3].championId.ToString();
                        Champion5Text.Text = MatchInfo.participants[4].championId.ToString();
                        Team2Text.Text = MatchInfo.teams[1].win.ToString();
                        Champion6Text.Text = MatchInfo.participants[5].championId.ToString();
                        Champion7Text.Text = MatchInfo.participants[6].championId.ToString();
                        Champion8Text.Text = MatchInfo.participants[7].championId.ToString();
                        Champion9Text.Text = MatchInfo.participants[8].championId.ToString();
                        Champion10Text.Text = MatchInfo.participants[9].championId.ToString();

                        //up
                        CheckInsertSQL(champion1, champion2, champion3, champion4, champion5, team1ID, win1, lose1);
                        CheckInsertSQL(champion6, champion7, champion8, champion9, champion10, team2ID, win2, lose2);
                    }
                    else
                    {
                        LabelAPIResponse.Text = "Non Classic Game";
                    } 
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Status Code : {0}", ((HttpWebResponse)ex.Response).StatusCode);
                    //Console.WriteLine("Status Description : {0}", ((HttpWebResponse)ex.Response).StatusDescription);
                    LabelAPIResponse.Text = ex.Message;
                }

                //go to next match.
                MatchID += 1;
                Console.WriteLine("Status Code : {0}", MatchID);
                CurrentMatchID.Text = MatchID.ToString();
                //can only read from api once per 2 seconds with current status
                System.Threading.Thread.Sleep(2000);
            }
        }

        private void CheckInsertSQL(int champion1, int champion2, int champion3, int champion4, int champion5, int team1ID, int win1, int lose1)
        {
            //query if a team exist; order doesnt matter
            string queryString = "SELECT * FROM [dbo].[TeamComps] " +
            "WHERE (Champion1 = @Champ1 OR Champion2 = @Champ1 OR Champion3 = @Champ1 OR Champion4 = @Champ1 OR Champion5 = @Champ1) " +
            "AND(Champion1 = @Champ2 OR Champion2 = @Champ2 OR Champion3 = @Champ2 OR Champion4 = @Champ2 OR Champion5 = @Champ2) " +
            "AND(Champion1 = @Champ3 OR Champion2 = @Champ3 OR Champion3 = @Champ3 OR Champion4 = @Champ3 OR Champion5 = @Champ3) " +
            "AND(Champion1 = @Champ4 OR Champion2 = @Champ4 OR Champion3 = @Champ4 OR Champion4 = @Champ4 OR Champion5 = @Champ4) " +
            "AND(Champion1 = @Champ5 OR Champion2 = @Champ5 OR Champion3 = @Champ5 OR Champion4 = @Champ5 OR Champion5 = @Champ5) ";

            using (SqlConnection connection = new SqlConnection(dataBaseConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.AddWithValue("@Champ1", champion1);
                command.Parameters.AddWithValue("@Champ2", champion2);
                command.Parameters.AddWithValue("@Champ3", champion3);
                command.Parameters.AddWithValue("@Champ4", champion4);
                command.Parameters.AddWithValue("@Champ5", champion5);

                SqlDataReader SQLreader = command.ExecuteReader();

                //only should be one team per location; if count = 0 than need to insert
                int count = 0;
                try
                {
                    while (SQLreader.Read())
                    {
                        count++;

                        team1ID = Convert.ToInt32(SQLreader["TeamID"].ToString());
                        win1 += Convert.ToInt32(SQLreader["Wins"].ToString());
                        lose1 += Convert.ToInt32(SQLreader["Losses"].ToString());
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    SQLreader.Close();
                }
                //update table if teamcomp was found
                if (count != 0)
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = "UPDATE TeamComps SET Wins = @Wins, Losses = @Losses WHERE TeamID = @teamID";
                    cmd.Parameters.AddWithValue("@Wins", win1);
                    cmd.Parameters.AddWithValue("@Losses", lose1);
                    cmd.Parameters.AddWithValue("@teamID", team1ID);
                    cmd.ExecuteNonQuery();
                }
                //add row if team comp not found
                else
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = "INSERT INTO TeamComps(Champion1, Champion2, Champion3, Champion4, Champion5, Wins, Losses) VALUES (@Champion1, @Champion2, @Champion3, @Champion4, @Champion5, @Wins, @Losses)";
                    cmd.Parameters.AddWithValue("@Champion1", champion1);
                    cmd.Parameters.AddWithValue("@Champion2", champion2);
                    cmd.Parameters.AddWithValue("@Champion3", champion3);
                    cmd.Parameters.AddWithValue("@Champion4", champion4);
                    cmd.Parameters.AddWithValue("@Champion5", champion5);
                    cmd.Parameters.AddWithValue("@Wins", win1);
                    cmd.Parameters.AddWithValue("@Losses", lose1);
                    cmd.ExecuteNonQuery();
                }
            }
        } 
    }
}
