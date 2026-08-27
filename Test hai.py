scores = [45, 88, 32, 91, 60, 15, 78]
passed_scores = []
k = m = 0 
for score in scores:
    if score >= 50:
        k += 1
        m = m+ score
        print("passss haiii", score)
        passed_scores.append(score)
print("Total passed:", k)
print("Total marks of passed students:", m)
print('pass bche', passed_scores )
p = sum(passed_scores)/ len (passed_scores)
print("Average marks of passed students:", p)