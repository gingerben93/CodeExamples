#finds all palindromes in a string done recursively
def Palindromes(string):
    start = 0
    end = len(string) - 1
    while  start < end:
        if string[start] != string[end]:
            return False
        start+=1
        end-=1
    return True
    
def FindPartition(string, size, result):
    for i in range(1, size+1):
        if Palindromes(string[:i]):
            if i == size:
                print(result + string[:i])
                break
            FindPartition(string[i:size-i + 1],size-i, result + string[:i] + " ")

string =  "nitinin"
test = FindPartition(string, len(string), "")