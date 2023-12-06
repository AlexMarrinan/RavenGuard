using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Array2D<T> {
    private int height;
    private int width;
    public T[] grid;

    public int Height { get => height; set => height = value; }
    public int Width { get => width; set => width = value; }
    public int Length { get => width*height; }
    
    public Array2D(int width, int height){
        this.width = width;
        this.height = height;
        grid = new T[width*height];
    }
    public void Set(int x, int y, T value){
        //Flips height 
        //(bottom left corner is 0,0 in game);
        //(top left corner is 0,0 in editor);
        int newy = this.height-1-y;
        grid[newy*width+x] = value;
    }
    public T Get(Vector2 pos){
        return Get((int)pos.x, (int)pos.y);
    }
    public T Get(int x, int y){
        int newy = this.height-1-y;
        return grid[newy*width+x];
    }
    public void Rotate()
    {   
        Debug.Log("Rotated");
        var newGrid = new T[grid.Length];
        
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int newy = height-1-y;
                T t = grid[newy*width+x];
                int newx = width-1-x;
                newGrid[newx*height+y] = t;
            }
        }

        int oldHeight = height;
        height = width;
        width = oldHeight;
        grid = newGrid;
    }

    public void FlipX()
    {   
        Debug.Log("X Flipped");
        var newGrid = new T[grid.Length];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int newy = height-1-y;
                T t = grid[newy*width+x];
                int newx = width-1-x;
                newGrid[newy*width+newx] = t;
            }
        }

        grid = newGrid;
    }

    public void FlipY()
    {
        Debug.Log("Y Flipped");
        var newGrid = new T[grid.Length];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int newy = height-1-y;
                T t = grid[newy*width+x];
                newGrid[y*width+x] = t;
            }
        }

        grid = newGrid;
    }

    internal void DeepCopy(Array2D<T> otherArray)
    {
        var otherGrid = otherArray.grid;
        grid = new T[otherGrid.Length];
        for (int i = 0; i < grid.Length; i++){
            grid[i] = otherGrid[i];
        }
        // height = otherArray.Height;
        // width = otherArray.Width;
    }
}