using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LeetCodeTests : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int[] TwoSum(int[] nums, int target)
    {
        Dictionary<int,int> numsMap = new Dictionary<int, int>();
        for(int i = 0; i < nums.Length; i++)
        {
            int complement = target - nums[i];
            if (numsMap.ContainsKey(complement))
            {
                return new int[] { numsMap[complement], i };
            }
            numsMap[nums[i]] = i;
        }
        return null; 


       
    }

    public bool IsPalindrome(int x)
    {
        if (x < 0 || (x % 10 == 0 && x != 0))
        {
            return false;
        }
        int reversed = 0;
        while (x > reversed)
        {
            int digit = x % 10;
            reversed = reversed * 10 + digit;
            x /= 10;
        }
        return x == reversed || x == reversed / 10;
    }

    public int RomanToInt(string s)
    {
        Dictionary<char,int> romandDico = new Dictionary<char, int>()
        {
            {'I', 1 },
            {'V', 5 },
            {'X', 10 },
            {'L', 50 },
            {'C', 100 },
            {'D', 500 },
            {'M', 1000 }
        };

        int total = 0;
        int prevValue = 0; 

        for (int i = s.Length-1; i >= 0 ; i--)
        {
            var currentValue = romandDico[s[i]];

            if(currentValue < prevValue)
            {
                total -= currentValue;
            }
            else
            {
                total += currentValue;
            }

            prevValue = currentValue; 

        }


        return total; 
    }

    public string LongestCommonPrefix(string[] strs)
    {
        if (strs.Length <= 0) return "";
        if (string.IsNullOrEmpty(strs[0])) return "";
        if (strs.Length <= 1) return strs[0];


        StringBuilder newPrefix2 = new StringBuilder();


        int whileIndex = 0; 
        while (whileIndex < strs[0].Length && EvenChars(strs, whileIndex))
        {
            newPrefix2.Append(strs[0][whileIndex]);
            whileIndex++; 
        }
      


        return newPrefix2.ToString(); 

    }

    bool EvenChars(string[] strs,int index)
    {
        
        char wordChar = strs[0][index];
        for (int i = 1; i < strs.Length; i++)
        {
            if (index >= strs[i].Length) return false;

            if (strs[i][index] != wordChar)
            {
                return false; 

            }


        }
        return true;
    }

    public bool IsValid(string s)
    {
        if (s == null || s.Length <= 0 ) return false; 
        Dictionary<char, char> bracketsDico = new Dictionary<char, char>()
        {
            { '(', ')' },
            { '{', '}' },
            { '[', ']' }
        };

        for (int i = 0; i < s.Length/2; i+=2)
        {

            if (s[s.Length-i-1] != bracketsDico[s[i]]) return false; 
        }

        return true; 
    }

    public class ListNode
    {
      public int val;
      public ListNode next;
      public ListNode(int val = 0, ListNode next = null)
        {
            this.val = val;
            this.next = next;

          }
  }

    public ListNode MergeTwoLists(ListNode list1, ListNode list2)
    {
        ListNode head = new ListNode(0);
        ListNode current = head;

        while (list1 !=null && list2 != null)
        {
            if(list1.val <= list2.val)
            {
                current.next = list1;
                list1 = list1.next;

            }
            else
            {
                current.next = list2;
                list2 = list2.next;

            }
            current = current.next;
        }
        if (list1 != null)
        {
            current.next = list1;
        }
        else if (list2 != null)
        {
            current.next = list2;
        }

        // The merged list starts after the dummy head
        return head.next;
    }

    public int RemoveDuplicates(int[] nums)
    {
        if (nums.Length == 0)
        {
            return 0;
        }

        // The slow pointer marks the position of the last unique element found.
        int slow = 0;

        // The fast pointer iterates through the array to find new unique elements.
        for (int fast = 1; fast < nums.Length; fast++)
        {
            // If a new unique element is found (nums[fast] is different from nums[slow]),
            // we place it at the next available position for unique elements.
            if (nums[fast] != nums[slow])
            {
                slow++;
                nums[slow] = nums[fast];
            }
        }

        // The number of unique elements is the index of the last unique element + 1.
        return slow + 1;
    }

    public int RemoveElement(int[] nums, int val)
    {
        int wrongElemNumber = 0; 

        int lastIndex = 0; 

        

        for (int i = 0; i < nums.Length; i++)
        {
            if(nums[i] != val)
            {
                wrongElemNumber++;
                nums[lastIndex] = nums[i];  
                lastIndex++;
            }
        }

        return wrongElemNumber;
        //got idea by replacing by the last element in the list 
    }

    public int StrStr(string haystack, string needle)
    {
        if (!haystack.Contains(needle)) return -1;

        if (haystack.Length < needle.Length) return -1; 

        int firstOccurence = 0;

        for (int i = 0;i < haystack.Length; i++)
        {
            if(haystack[i] == needle[0])
            {
                firstOccurence = i; 
                if(haystack.Length == 1 || needle.Length ==1 ) return firstOccurence; 

                for (int j = 1; j < needle.Length; j++)
                {
                    if (haystack[j + i] == needle[j] && j == needle.Length - 1) return firstOccurence; 
                    if (haystack[j+i] == needle[j])
                    {
                        continue;
                    }
                    else
                    {
                        break; 
                    }
                }
            }

        }
        return -1;  


    }

    public int SearchInsert(int[] nums, int target)
    {
        int index = 0; 
        for (int i = 0; i < nums.Length; i++)
        {
            
            if(nums[i] == target)
            {
                return i; 
            }
            if (nums[i] < target)
            {
                index = i +1 ;

            }
        }
        if (target < nums[0]) return 0; 
        return index ;
    }

    public int LengthOfLastWord(string s)
    {
        int letterNumber = 0;
        bool wordStarted = false;
        if (s.Length <= 0) return 0; 
        if (s.Length == 1) return 1; 
        for (int i = s.Length-1; i >= 0 ; i--)
        {
            if (s[i] !=  ' ')
            {
                wordStarted = true;
                letterNumber++;
            }
            else if (s[i] == ' ' && wordStarted)
            {
                break; 
            }
               
        }

        return letterNumber;

    }


    public int[] PlusOne(int[] digits)
    {
        // Iterate from the last digit to the first.
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            // If the current digit is not 9, we can simply increment it and return the array.
            // No carry is needed.
            if (digits[i] < 9)
            {
                digits[i]++;
                return digits;
            }

            // If the digit is 9, we set it to 0 and the loop will handle the carry to the next digit.
            digits[i] = 0;
        }

        // If the loop completes, it means the number was something like [9,9,9].
        // We need to create a new array with an extra digit at the beginning.
        int[] newDigits = new int[digits.Length + 1];
        newDigits[0] = 1;
        // The rest of the new array is already initialized to 0.

        return newDigits;
    }

    public string AddBinary(string a, string b)
    {
        return (int.Parse(a) ^ int.Parse(b)).ToString();
        
    }

    public int MySqrt(int x)
    {
        if (x == 0)
        {
            return 0;
        }

        long low = 1;
        long high = x;
        long result = 0;

        while (low <= high)
        {
            long mid = low + (high - low) / 2;
            long square = mid * mid;

            if (square == x)
            {
                return (int)mid;
            }
            else if (square < x)
            {
                // This could be the answer, but check the upper half for a larger value.
                result = mid;
                low = mid + 1;
            }
            else
            {
                // Too large, search the lower half.
                high = mid - 1;
            }
        }

        return (int)result;
    }

    public int ClimbStairs(int n)
    {
        if (n <= 2)
        {
            return n;
        }

        int prev1 = 1; // Represents the number of ways to climb 1 step
        int prev2 = 2; // Represents the number of ways to climb 2 steps

        for (int i = 3; i <= n; i++)
        {
            int current = prev1 + prev2;
            prev1 = prev2;
            prev2 = current;
        }

        return prev2;
    }
}



